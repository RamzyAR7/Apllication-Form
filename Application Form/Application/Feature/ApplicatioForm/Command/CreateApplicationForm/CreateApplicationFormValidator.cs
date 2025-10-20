using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net;
using System.Net.Mail;
using Application_Form.Domain.Constant;
using FluentValidation;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm
{
    public class CreateApplicationFormValidator : AbstractValidator<CreateApplicationFormCommand>
    {
        public CreateApplicationFormValidator()
        {
            RuleFor(x => x.Dto.ApplicationName)
                .NotEmpty().WithMessage("ApplicationName is required.")
                .MaximumLength(100).WithMessage("ApplicationName must be at most 100 characters.");

            RuleFor(x => x.Dto.ApplicationDescription)
                .NotEmpty().WithMessage("ApplicationDescription is required.")
                .MaximumLength(1000).WithMessage("ApplicationDescription must be at most 1000 characters.");

            RuleFor(x => x.Dto.EmailAddress)
                .NotEmpty().WithMessage("EmailAddress is required.")
                .EmailAddress().WithMessage("EmailAddress must be a valid email address.")
                .MaximumLength(255).WithMessage("EmailAddress must be at most 255 characters.")
                .MustAsync(async (command, email, ct) =>
                {
                    try
                    {
                        var host = new MailAddress(email).Host;
                        await Dns.GetHostEntryAsync(host);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }).WithMessage("Email domain does not exist.");

            RuleFor(x => x.Dto.ApplicationType)
                .NotEmpty().WithMessage("ApplicationType is required.")
                .MaximumLength(50).WithMessage("ApplicationType must be at most 50 characters.");

            RuleFor(x => x.Dto.RedirectUri)
                .MaximumLength(500).WithMessage("RedirectUri must be at most 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.RedirectUri));

            RuleFor(x => x.Dto.RedirectUri)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("RedirectUri must be a valid URL.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.RedirectUri));

            RuleFor(x => x.Dto.Environment)
                .NotEmpty().WithMessage("Environment is required.")
                .MaximumLength(50).WithMessage("Environment must be at most 50 characters.")
                .Must(env => Enum.GetNames(typeof(ApiEnvironment)).Contains(env))
                .WithMessage("Invalid environment. Must be Sandbox, Production, or Both.");

            RuleFor(x => x.Dto.ExpectedRequestVolume)
                .GreaterThanOrEqualTo(0).WithMessage("ExpectedRequestVolume must be a non-negative integer.")
                .When(x => x.Dto.ExpectedRequestVolume.HasValue);

            RuleFor(x => x.Dto.AcceptTerms)
                .Equal(true).WithMessage("You must accept the terms.");

            RuleFor(x => x.Dto.PrivacyPolicyUrl)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(300).WithMessage("PrivacyPolicyUrl must be at most 300 characters.")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).WithMessage("PrivacyPolicyUrl must be a valid URL.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.PrivacyPolicyUrl));

            RuleFor(x => x.Dto.TechnicalContactName)
                .MaximumLength(100).WithMessage("TechnicalContactName must be at most 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.TechnicalContactName));

            RuleFor(x => x.Dto.TechnicalContactEmail)
                .EmailAddress().WithMessage("TechnicalContactEmail must be a valid email address.")
                .MaximumLength(150).WithMessage("TechnicalContactEmail must be at most 150 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.TechnicalContactEmail))
                .MustAsync(async (command, email, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(email)) return true;
                    try
                    {
                        var host = new MailAddress(email).Host;
                        await Dns.GetHostEntryAsync(host);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }).WithMessage("Technical contact email domain does not exist.");

            RuleFor(x => x.Dto.OrganizationName)
                .MaximumLength(150).WithMessage("OrganizationName must be at most 150 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.OrganizationName));

            RuleFor(x => x.Dto.DataRetentionDescription)
                .MaximumLength(500).WithMessage("DataRetentionDescription must be at most 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.DataRetentionDescription));

            RuleFor(x => x.Dto.ClientId)
                .NotEmpty().WithMessage("ClientId is required.");
        }
    }
}
