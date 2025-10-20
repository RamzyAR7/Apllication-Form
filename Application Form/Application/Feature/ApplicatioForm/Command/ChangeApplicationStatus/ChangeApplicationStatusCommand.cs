using Application_Form.Application.DTOs;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.ChangeApplicationStatus
{
    public class ChangeApplicationStatusCommand: IRequest<Result<ApplicationFormListResponseDto>>
    {
        public Guid Id { get; set; }
        public ChangeApplicationStatusDto Dto { get; set; }
        public ChangeApplicationStatusCommand(Guid id, ChangeApplicationStatusDto dto)
        {
            Id = id;
            Dto = dto;
        }
    }
}
