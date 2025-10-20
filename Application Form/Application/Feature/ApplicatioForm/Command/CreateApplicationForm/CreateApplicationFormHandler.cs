using Application_Form.Application.DTOs;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Common;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm
{
    public class CreateApplicationFormHandler : IRequestHandler<CreateApplicationFormCommand, Result<CustomEmptyResult>>
    {
        private readonly IApplicationFormRepository _repository;
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateApplicationFormHandler> _logger;

        public CreateApplicationFormHandler(IApplicationFormRepository repository, IClientRepository clientRepository, IMapper mapper, ILogger<CreateApplicationFormHandler> logger)
        {
            _repository = repository;
            _clientRepository = clientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CustomEmptyResult>> Handle(CreateApplicationFormCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating application form for client {ClientId}", request.Dto?.ClientId);
            try
            {
                var client = await _clientRepository.GetByIdAsync(request.Dto.ClientId);
                if (client == null)
                {
                    _logger.LogWarning("Client {ClientId} not found", request.Dto.ClientId);
                    return Result<CustomEmptyResult>.Failure("Client not found.");
                }
                var existedApp = await _repository.GetByNameAndClientIdAsync(request.Dto.ApplicationName, request.Dto.ClientId);

                if(existedApp != null)
                {
                    _logger.LogWarning("Application with name {AppName} already exists for client {ClientId}", request.Dto.ApplicationName, request.Dto.ClientId);
                    return Result<CustomEmptyResult>.Failure("An application with the same name already exists for this client.");
                }

                var entity = _mapper.Map<ApplicationForm>(request.Dto);

                // seed ApiDocsUrl
                entity.ApiDocsUrl = "https://api.example.com/docs/";

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Created application {AppId}", entity.Id);
                return Result<CustomEmptyResult>.SuccessResult(new CustomEmptyResult());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating application for client {ClientId}", request.Dto?.ClientId);
                return Result<CustomEmptyResult>.Failure($"Error creating application: {ex.Message}");
            }
        }
    }
}
