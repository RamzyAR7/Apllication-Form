using System;
using System.Linq;
using FluentValidation;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.ChangeApplicationStatus
{
    public class ChangeApplicationStatusVaildator : AbstractValidator<ChangeApplicationStatusCommand>
    {
        public ChangeApplicationStatusVaildator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Application ID is required.");

            RuleFor(x => x.Dto)
                .NotNull().WithMessage("Request body is required.");

            When(x => x.Dto != null, () =>
            {
                var allowedStatuses = Enum.GetNames(typeof(Status)).Where(n => n != nameof(Status.Pending));
                var allowedPattern = $"^({string.Join("|", allowedStatuses)})$";

                RuleFor(x => x.Dto.NewStatus)
                    .NotEmpty().WithMessage("New Status is required.")
                    .Matches(allowedPattern).WithMessage($"NewStatus must be one of: {string.Join(", ", allowedStatuses)}.");

                RuleFor(x => x.Dto.ExpirationDate)
                    .Must(date => date == null || date.Value > DateOnly.FromDateTime(DateTime.UtcNow))
                    .WithMessage("Expiration date must be in the future.")
                    .When(x => x.Dto.ExpirationDate.HasValue);
            });
        }
    }
}
