using Application_Form.Application.DTOs;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.DeleteApplication
{
    public class DeleteApplicationCommand : IRequest<Result<CustomEmptyResult>>
    {
        public long Id { get; set; }

        public DeleteApplicationCommand(long id)
        {
            Id = id;
        }
    }
}
