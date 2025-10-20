using Application_Form.Application.DTOs;
using Application_Form.Domain.Common;
using MediatR;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm
{
    public class CreateApplicationFormCommand : IRequest<Result<CustomEmptyResult>>
    {
        public CreateApplicationFormDto Dto { get; set; }

        public CreateApplicationFormCommand(CreateApplicationFormDto dto)
        {
            Dto = dto;
        }
    }
}
