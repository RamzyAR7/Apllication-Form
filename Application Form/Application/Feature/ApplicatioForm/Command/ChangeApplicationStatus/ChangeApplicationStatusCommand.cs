using Application_Form.Application.DTOs;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.ChangeApplicationStatus
{
    public class ChangeApplicationStatusCommand: IRequest<Result<CustomEmptyResult>>
    {
        public long Id { get; set; }
        public ChangeApplicationStatusDto Dto { get; set; }
        public ChangeApplicationStatusCommand(long id, ChangeApplicationStatusDto dto)
        {
            Id = id;
            Dto = dto;
        }
    }
}
