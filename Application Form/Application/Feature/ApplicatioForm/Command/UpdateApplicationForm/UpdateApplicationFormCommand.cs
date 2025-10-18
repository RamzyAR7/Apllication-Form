using Application_Form.Application.DTOs;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.UpdateApplicationForm
{
    public class UpdateApplicationFormCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public UpdateApplicationFormDto Dto { get; set; }

        public UpdateApplicationFormCommand(Guid id, UpdateApplicationFormDto dto)
        {
            Id = id;
            Dto = dto;
        }
    }
}
