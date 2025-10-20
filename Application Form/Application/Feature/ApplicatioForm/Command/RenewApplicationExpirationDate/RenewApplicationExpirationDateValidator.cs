using FluentValidation;
using System;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationExpirationDate
{
    public class RenewApplicationExpirationDateValidator: AbstractValidator<RenewApplicationExpirationDateCommand>
    {
        public RenewApplicationExpirationDateValidator()
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithMessage("ApplicationId is required.");
            RuleFor(x => x.NewExpirationDate)
                .NotEmpty().WithMessage("New expiration date is required.")
                .Must(date => date > DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("New expiration date must be in the future.");
        }
    }
}
