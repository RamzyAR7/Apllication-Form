using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.BulkDeleteApplications
{
    public class BulkDeleteApplicationsHandler : IRequestHandler<BulkDeleteApplicationsCommand, Result>
    {
        private readonly IApplicationFormRepository _repository;

        public BulkDeleteApplicationsHandler(IApplicationFormRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(BulkDeleteApplicationsCommand request, CancellationToken cancellationToken)
        {
            if (request?.Ids == null || !request.Ids.Any())
                return Result.Failure("No IDs provided for bulk delete.");

            foreach (var id in request.Ids)
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null || entity.IsDeleted) continue;
                entity.IsDeleted = true;
                entity.IsActive = false;
                entity.LastModified = DateTime.UtcNow;
                _repository.Update(entity);
            }

            await _repository.SaveChangesAsync();

            return Result.SuccessResult();
        }
    }
}
