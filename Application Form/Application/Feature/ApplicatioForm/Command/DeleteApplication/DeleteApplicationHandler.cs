using Application_Form.Application.DTOs;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.DeleteApplication
{
    public class DeleteApplicationHandler : IRequestHandler<DeleteApplicationCommand, Result<ApplicationFormListResponseDto>>
    {
        private readonly IApplicationFormRepository _repository;
        private readonly ILogger<DeleteApplicationHandler> _logger;

        public DeleteApplicationHandler(IApplicationFormRepository repository, ILogger<DeleteApplicationHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<ApplicationFormListResponseDto>> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting application {AppId}", request.Id);
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null || entity.IsDeleted)
            {
                _logger.LogWarning("Application {AppId} not found or already deleted", request.Id);
                return Result<ApplicationFormListResponseDto>.Failure("Application not found.");
            }

            entity.IsDeleted = true;
            entity.IsActive = false;
            entity.LastModified = DateTime.UtcNow;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Application {AppId} marked as deleted", request.Id);
            return Result<ApplicationFormListResponseDto>.SuccessResult(null);
        }
    }
}
