using Application_Form.Application.DTOs;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.DeleteApplication
{
    public class DeleteApplicationCommand : IRequest<Result<ApplicationFormListResponseDto>>
    {
        public Guid Id { get; set; }

        public DeleteApplicationCommand(Guid id)
        {
            Id = id;
        }
    }
}
