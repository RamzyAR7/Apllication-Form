using Application_Form.Application.DTOs;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Common;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm
{
    public class CreateApplicationFormHandler : IRequestHandler<CreateApplicationFormCommand, Result>
    {
        private readonly IApplicationFormRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateApplicationFormHandler> _logger;

        public CreateApplicationFormHandler(IApplicationFormRepository repository, IMapper mapper, ILogger<CreateApplicationFormHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateApplicationFormCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating application form for client {ClientId}", request.Dto?.ClientId);
            try
            {
                // Map from DTO → Entity
                var entity = _mapper.Map<ApplicationForm>(request.Dto);

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Created application {AppId}", entity.Id);
                return Result.SuccessResult(string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating application for client {ClientId}", request.Dto?.ClientId);
                return Result.Failure($"Error creating application: {ex.Message}");
            }
        }
    }
}
