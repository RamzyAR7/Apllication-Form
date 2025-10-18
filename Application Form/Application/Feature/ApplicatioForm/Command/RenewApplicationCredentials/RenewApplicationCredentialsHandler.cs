using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Application.Services;
using Application_Form.Domain.Common;
using Application_Form.Domain.Constant;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationCredentials
{
    public class RenewApplicationCredentialsHandler : IRequestHandler<RenewApplicationCredentialsCommand, Result>
    {
        private readonly IApplicationFormRepository _repository;
        private readonly IApiCredentialService _apiCredentialService;
        private readonly ILogger<RenewApplicationCredentialsHandler> _logger;

        public RenewApplicationCredentialsHandler(IApplicationFormRepository repository, IApiCredentialService apiCredentialService, ILogger<RenewApplicationCredentialsHandler> logger)
        {
            _repository = repository;
            _apiCredentialService = apiCredentialService;
            _logger = logger;
        }

        public async Task<Result> Handle(RenewApplicationCredentialsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Renewing credentials for application {AppId}", request.Id);
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null || entity.IsDeleted)
            {
                _logger.LogWarning("Application {AppId} not found or deleted", request.Id);
                return Result.Failure("Application not found.");
            }

            // Must be active and approved
            if (!entity.IsActive)
            {
                _logger.LogWarning("Attempt to renew credentials for inactive application {AppId}", request.Id);
                return Result.Failure("Application must be active to renew credentials.");
            }

            if (!string.Equals(entity.ApprovalStatus, Status.Approved.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Attempt to renew credentials for non-approved application {AppId} with status {Status}", request.Id, entity.ApprovalStatus);
                return Result.Failure("Only approved applications can renew credentials.");
            }

            // If has expiration date, must not be expired
            if (entity.ExpirationDate.HasValue && entity.ExpirationDate.Value < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                _logger.LogWarning("Attempt to renew credentials for expired application {AppId}", request.Id);
                return Result.Failure("Application is expired and cannot renew credentials.");
            }

            try
            {
                // Generate new credentials
                var creds = await _apiCredentialService.GenerateAsync(cancellationToken);
                entity.ApiKey = creds.ApiKey;
                entity.ApiClientId = creds.ApiClientId;
                entity.ApiClientSecret = creds.ApiClientSecret;
                entity.LastModified = DateTime.UtcNow;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Credentials renewed for application {AppId}", request.Id);
                return Result.SuccessResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing credentials for application {AppId}", request.Id);
                return Result.Failure($"Error renewing credentials: {ex.Message}");
            }
        }
    }
}
