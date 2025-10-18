using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.BulkDeleteApplications
{
    public class BulkDeleteApplicationsCommand : IRequest<Result>
    {
        public IEnumerable<Guid> Ids { get; set; }

        public BulkDeleteApplicationsCommand(IEnumerable<Guid> ids)
        {
            Ids = ids;
        }
    }
}
