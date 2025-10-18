using FluentValidation;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.DeleteApplication
{
    public class DeleteApplicationValidator : AbstractValidator<DeleteApplicationCommand>
    {
        public DeleteApplicationValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Application ID is required.");
        }
    }
}
