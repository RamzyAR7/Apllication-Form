using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Common;
using Application_Form.Domain.Constant;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System;
using Application_Form.Application.DTOs;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationExpirationDate
{
    public class RenewApplicationExpirationDateHandler : IRequestHandler<RenewApplicationExpirationDateCommand, Result<CustomEmptyResult>>
    {
        private readonly IApplicationFormRepository _applicationRepository;
        private readonly ILogger<RenewApplicationExpirationDateHandler> _logger;

        public RenewApplicationExpirationDateHandler(IApplicationFormRepository applicationRepository, ILogger<RenewApplicationExpirationDateHandler> logger)
        {
            _applicationRepository = applicationRepository;
            _logger = logger;
        }

        public async Task<Result<CustomEmptyResult>> Handle(RenewApplicationExpirationDateCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting renewal process for ApplicationId: {AppId}", request.ApplicationId);

            var entity = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            if (entity == null)
            {
                _logger.LogWarning("Application not found. Id: {AppId}", request.ApplicationId);
                return Result<CustomEmptyResult>.Failure("Application not found.");
            }

            if (entity.ApprovalStatus != Status.Approved.ToString() && entity.ApprovalStatus != Status.Expired.ToString())
            {
                _logger.LogWarning("Renewal denied for ApplicationId: {AppId} (Status: {Status})", request.ApplicationId, entity.ApprovalStatus);
                return Result<CustomEmptyResult>.Failure("Only approved or Expired applications can be renewed.");
            }

            var newDate = request.NewExpirationDate;
            var now = DateOnly.FromDateTime(DateTime.UtcNow);

            if (newDate <= now)
            {
                _logger.LogWarning("Invalid expiration date provided for ApplicationId: {AppId}", request.ApplicationId);
                return Result<CustomEmptyResult>.Failure("Expiration date must be a future date.");
            }

            var oldDate = entity.ExpirationDate;

            if (!oldDate.HasValue)
            {
                _logger.LogInformation("ApplicationId: {AppId} has no expiration date — assigning new expiration {NewDate}", request.ApplicationId, newDate);
            }
            else if (oldDate.Value < now)
            {
                _logger.LogInformation("ApplicationId: {AppId} expired on {OldDate} — renewing to {NewDate}", request.ApplicationId, oldDate, newDate);
                entity.IsActive = true;
                entity.ApprovalStatus = Status.Approved.ToString();
            }
            else
            {
                _logger.LogInformation("ApplicationId: {AppId} active — extending expiration from {OldDate} to {NewDate}", request.ApplicationId, oldDate, newDate);
            }

            entity.ExpirationDate = newDate;
            entity.LastModified = DateTime.UtcNow;

            _applicationRepository.Update(entity);
            await _applicationRepository.SaveChangesAsync();

            _logger.LogInformation("ApplicationId: {AppId} renewed successfully until {Date}.", request.ApplicationId, entity.ExpirationDate);

            return Result<CustomEmptyResult>.SuccessResult(new CustomEmptyResult());
        }
    }
}
