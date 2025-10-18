using FluentValidation;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationCredentials
{
    public class RenewApplicationCredentialsValidator : AbstractValidator<RenewApplicationCredentialsCommand>
    {
        public RenewApplicationCredentialsValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Application ID is required.");
        }
    }
}
