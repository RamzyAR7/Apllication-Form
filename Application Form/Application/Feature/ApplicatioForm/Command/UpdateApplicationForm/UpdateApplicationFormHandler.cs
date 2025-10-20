using Application_Form.Application.DTOs;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Common;
using Application_Form.Domain.Constant;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.UpdateApplicationForm
{
    public class UpdateApplicationFormHandler : IRequestHandler<UpdateApplicationFormCommand, Result<CustomEmptyResult>>
    {
        private readonly IApplicationFormRepository _repository;
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateApplicationFormHandler> _logger;

        public UpdateApplicationFormHandler(IApplicationFormRepository repository,IClientRepository clientRepository, IMapper mapper, ILogger<UpdateApplicationFormHandler> logger)
        {
            _repository = repository;
            _clientRepository = clientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CustomEmptyResult>> Handle(UpdateApplicationFormCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating application {AppId}", request.Id);
            try
            {
                var entity = await _repository.GetByIdAsync(request.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Application {AppId} not found or deleted", request.Id);
                    return Result<CustomEmptyResult>.Failure("Application not found.");
                }
                var isClientExists = await _clientRepository.GetByIdAsync(request.Dto.ClientId);
                if (isClientExists == null)
                {
                    _logger.LogWarning("Client {ClientId} not found for application {AppId}", request.Dto.ClientId, request.Id);
                    return Result<CustomEmptyResult>.Failure("The client does not exist.");
                }

                if (entity.ApprovalStatus != Status.Pending.ToString())
                {
                    _logger.LogWarning("Attempt to update application {AppId} with status {Status}", request.Id, entity.ApprovalStatus);
                    return Result<CustomEmptyResult>.Failure("Only Pending applications can be updated.");
                }
                if (entity.ClientId != request.Dto.ClientId)
                {
                    _logger.LogWarning("Attempt to change ClientId for application {AppId}", request.Id);
                    return Result<CustomEmptyResult>.Failure("The client does not own this application.");
                }

                // Ensure no other application with same name exists for this client
                var existedPage = await _repository.GetPagedByClientIdAsync(request.Dto.ClientId, 1, 1000, "CreatedAt", "asc", "all");
                var existingApp = existedPage.Items.FirstOrDefault(a => !string.IsNullOrEmpty(a.ApplicationName) && string.Equals(a.ApplicationName.Trim(), request.Dto.ApplicationName?.Trim(), StringComparison.OrdinalIgnoreCase));
                if (existingApp != null && existingApp.Id != request.Id)
                {
                    _logger.LogWarning("Application name {AppName} already exists for client {ClientId}", request.Dto.ApplicationName, request.Dto.ClientId);
                    return Result<CustomEmptyResult>.Failure("An application with the same name already exists for this client.");
                }
                // Map update DTO onto existing entity; mapping is configured to ignore ClientId and system fields
                _mapper.Map(request.Dto, entity);

                entity.LastModified = DateTime.UtcNow;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Application {AppId} updated successfully", request.Id);
                return Result<CustomEmptyResult>.SuccessResult(new CustomEmptyResult());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application {AppId}", request.Id);
                return Result<CustomEmptyResult>.Failure($"Error updating application: {ex.Message}");
            }
        }
    }
}
