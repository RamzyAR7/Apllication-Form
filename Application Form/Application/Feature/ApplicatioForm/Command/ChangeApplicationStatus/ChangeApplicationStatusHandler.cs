using Application_Form.Application.DTOs;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Application.Services;
using Application_Form.Domain.Common;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Entities;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.ChangeApplicationStatus
{
    public class ChangeApplicationStatusHandler : IRequestHandler<ChangeApplicationStatusCommand, Result<CustomEmptyResult>>
    {
        private readonly IApplicationFormRepository _repository;
        private readonly IApiCredentialService _apiCredentialService;
        private readonly ILogger<ChangeApplicationStatusHandler> _logger;

        public ChangeApplicationStatusHandler(
            IApplicationFormRepository repository,
            IApiCredentialService apiCredentialService,
            ILogger<ChangeApplicationStatusHandler> logger)
        {
            _repository = repository;
            _apiCredentialService = apiCredentialService;
            _logger = logger;
        }

        public async Task<Result<CustomEmptyResult>> Handle(ChangeApplicationStatusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting status change for ApplicationId: {AppId} → Target: {TargetStatus}", request.Id, request.Dto?.NewStatus);

            try
            {
                var entity = await _repository.GetByIdAsync(request.Id);
                if (entity == null)
                {
                    _logger.LogWarning("Application not found. Id: {AppId}", request.Id);
                    return Result<CustomEmptyResult>.Failure("Application not found.");
                }
                if (entity.ExpirationDate <= DateOnly.FromDateTime(DateTime.UtcNow) && entity.IsActive == true)
                {
                    entity.IsActive = false;
                    entity.ApprovalStatus = Status.Expired.ToString();
                    _repository.Update(entity);
                    _logger.LogInformation("Application {AppId} has expired. Updated status to Expired and set IsActive to false.", request.Id);
                    await _repository.SaveChangesAsync();
                    return Result<CustomEmptyResult>.Failure("Application has expired - no status changes allowed.");
                }
                var current = entity.ApprovalStatus;
                var target = request.Dto?.NewStatus;

                // same status case
                if (string.Equals(current, target, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("ApplicationId: {AppId} already in status '{Status}', no change required.", request.Id, target);
                    return Result<CustomEmptyResult>.SuccessResult(new CustomEmptyResult());
                }

                bool isExpired = entity.ExpirationDate.HasValue && entity.ExpirationDate.Value < DateOnly.FromDateTime(DateTime.UtcNow);
                if (isExpired) {
                    _logger.LogInformation("ApplicationId: {AppId} is expired as of {ExpirationDate}. No status changes allowed.", request.Id, entity.ExpirationDate.Value);
                    return Result<CustomEmptyResult>.Failure("Application is expired; no status changes allowed.");
                }

                // check allowed transitions
                if (!IsTransitionAllowed(current, target))
                {
                    _logger.LogWarning("Invalid transition attempt from '{From}' to '{To}' for ApplicationId: {AppId}", current, target, request.Id);
                    return Result<CustomEmptyResult>.Failure($"Transition from '{current}' to '{target}' is not allowed.");
                }

                _logger.LogInformation("Processing transition from '{From}' to '{To}' for ApplicationId: {AppId}", current, target, request.Id);

                switch (target)
                {
                    case var s when s == Status.Approved.ToString():
                        await ApproveAsync(entity, request.Dto, cancellationToken);
                        _logger.LogInformation("ApplicationId: {AppId} approved successfully.", request.Id);
                        break;

                    case var s when s == Status.Rejected.ToString():
                        await RejectAsync(entity, request.Dto, cancellationToken);
                        _logger.LogInformation("ApplicationId: {AppId} rejected by admin. Notes: {Notes}", request.Id, request.Dto?.AdminNotes);
                        break;

                    case var s when s == Status.Revoked.ToString():
                        await RevokeAsync(entity, request.Dto, cancellationToken);
                        _logger.LogInformation("ApplicationId: {AppId} revoked by admin. Notes: {Notes}", request.Id, request.Dto?.AdminNotes);
                        break;

                    default:
                        _logger.LogWarning("Unsupported target status '{Target}' for ApplicationId: {AppId}", target, request.Id);
                        return Result<CustomEmptyResult>.Failure("Unsupported target status.");
                }

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("ApplicationId: {AppId} status successfully changed to '{Status}' at {Time}", request.Id, target, DateTime.UtcNow);

                return Result<CustomEmptyResult>.SuccessResult(new CustomEmptyResult());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing status for ApplicationId: {AppId}", request.Id);
                return Result<CustomEmptyResult>.Failure($"Change application status failed: {ex.Message}");
            }
        }

        private bool IsTransitionAllowed(string current, string? target)
        {
            if (string.IsNullOrWhiteSpace(target))
                return false;

            var allowedTransitions = new HashSet<(string From, string To)>
            {
                (Status.Pending.ToString(), Status.Approved.ToString()),
                (Status.Pending.ToString(), Status.Rejected.ToString()),
                (Status.Approved.ToString(), Status.Revoked.ToString())
            };

            return allowedTransitions.Contains((current, target));
        }

        private async Task ApproveAsync(ApplicationForm entity, ChangeApplicationStatusDto? dto, CancellationToken cancellationToken)
        {
            var creds = await _apiCredentialService.GenerateAsync(cancellationToken);
            entity.ApiKey = creds.ApiKey;
            entity.ApiClientId = creds.ApiClientId;
            entity.ApiClientSecret = creds.ApiClientSecret;

            entity.ApprovalStatus = Status.Approved.ToString();
            entity.IsActive = true;
            if (dto?.ExpirationDate.HasValue == true)
                entity.ExpirationDate = dto.ExpirationDate;
            entity.LastModified = DateTime.UtcNow;

            // TODO: send notification email
        }

        private Task RejectAsync(ApplicationForm entity, ChangeApplicationStatusDto? dto, CancellationToken cancellationToken)
        {
            entity.ApprovalStatus = Status.Rejected.ToString();
            entity.IsActive = false;
            entity.AdminNotes = dto?.AdminNotes;
            entity.LastModified = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        private Task RevokeAsync(ApplicationForm entity, ChangeApplicationStatusDto? dto, CancellationToken cancellationToken)
        {
            entity.ApprovalStatus = Status.Revoked.ToString();
            entity.IsActive = false;
            entity.AdminNotes = dto?.AdminNotes;
            entity.ApiKey = null;
            entity.ApiClientId = null;
            entity.ApiClientSecret = null;
            entity.LastModified = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}
