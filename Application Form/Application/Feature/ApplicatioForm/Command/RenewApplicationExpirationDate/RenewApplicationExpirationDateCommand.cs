using Application_Form.Application.DTOs;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationExpirationDate
{
    public class RenewApplicationExpirationDateCommand: IRequest<Result<ApplicationFormListResponseDto>>
    {
        public Guid ApplicationId { get; set; }
        public DateOnly NewExpirationDate { get; set; }

        public RenewApplicationExpirationDateCommand(Guid applicationId, DateOnly newExpirationDate)
        {
            ApplicationId = applicationId;
            NewExpirationDate = newExpirationDate;
        }
    }
}
