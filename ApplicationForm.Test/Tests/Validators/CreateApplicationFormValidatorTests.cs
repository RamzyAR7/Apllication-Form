using Xunit;
using FluentValidation.TestHelper;
using Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm;
using System;
using System.Threading.Tasks;
using Application_Form.Application.DTOs;

namespace ApplicationForm.Test.Tests.Validators
{
    public class CreateApplicationFormValidatorTests
    {
        private readonly CreateApplicationFormValidator _validator = new CreateApplicationFormValidator();

        private CreateApplicationFormCommand GetValidCommand()
        {
            return new CreateApplicationFormCommand(
                new CreateApplicationFormDto
                {
                    ApplicationName = "Test App",
                    ApplicationDescription = "A test application.",
                    EmailAddress = "test@example.com",
                    OrganizationName = "Test Org",
                    CountryId = 1,
                    ApplicationType = "Web",
                    RedirectUri = "https://example.com/callback",
                    Environment = "Sandbox",
                    ExpectedRequestVolume = 100,
                    AcceptTerms = true,
                    PrivacyPolicyUrl = "https://example.com/privacy",
                    DataRetentionDescription = "Data retained for 1 year.",
                    TechnicalContactName = "Tech Contact",
                    TechnicalContactEmail = "tech@example.com",
                    ClientId = Guid.NewGuid()
                }
            );
        }

        [Fact]
        public async Task Should_Have_Error_When_ApplicationName_Is_Empty()
        {
            var command = GetValidCommand();
            command.Dto.ApplicationName = string.Empty;
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.ApplicationName);
        }

        [Fact]
        public async Task Should_Have_Error_When_EmailAddress_Is_Invalid()
        {
            var command = GetValidCommand();
            command.Dto.EmailAddress = "not-an-email";
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.EmailAddress);
        }

        [Fact]
        public async Task Should_Have_Error_When_RedirectUri_Is_Invalid()
        {
            var command = GetValidCommand();
            command.Dto.RedirectUri = "not-a-url";
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.RedirectUri);
        }

        [Fact]
        public async Task Should_Have_Error_When_Environment_Is_Invalid()
        {
            var command = GetValidCommand();
            command.Dto.Environment = "InvalidEnv";
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.Environment);
        }

        [Fact]
        public async Task Should_Have_Error_When_AcceptTerms_Is_False()
        {
            var command = GetValidCommand();
            command.Dto.AcceptTerms = false;
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.AcceptTerms);
        }

        [Fact]
        public async Task Should_Not_Have_Any_Errors_For_Valid_Command()
        {
            var command = GetValidCommand();
            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Should_Have_Error_When_ApplicationName_Exceeds_MaxLength()
        {
            var command = GetValidCommand();
            command.Dto.ApplicationName = new string('A', 101);
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.ApplicationName);
        }

        [Fact]
        public async Task Should_Have_Error_When_ApplicationDescription_Exceeds_MaxLength()
        {
            var command = GetValidCommand();
            command.Dto.ApplicationDescription = new string('A', 1001);
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.ApplicationDescription);
        }

        [Fact]
        public async Task Should_Have_Error_When_EmailAddress_Exceeds_MaxLength()
        {
            var command = GetValidCommand();
            command.Dto.EmailAddress = new string('a', 250) + "@ex.com";
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.EmailAddress);
        }

        [Fact]
        public async Task Should_Have_Error_When_OrganizationName_Exceeds_MaxLength()
        {
            var command = GetValidCommand();
            command.Dto.OrganizationName = new string('O', 151);
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.OrganizationName);
        }

        [Fact]
        public async Task Should_Have_Error_When_ApplicationType_Is_Empty()
        {
            var command = GetValidCommand();
            command.Dto.ApplicationType = string.Empty;
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.ApplicationType);
        }

        [Fact]
        public async Task Should_Have_Error_When_ApplicationType_Exceeds_MaxLength()
        {
            var command = GetValidCommand();
            command.Dto.ApplicationType = new string('T', 51);
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.ApplicationType);
        }

        [Fact]
        public async Task Should_Have_Error_When_RedirectUri_Exceeds_MaxLength()
        {
            var command = GetValidCommand();
            command.Dto.RedirectUri = "https://" + new string('a', 495) + ".com";
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.RedirectUri);
        }

        [Fact]
        public async Task Should_Have_Error_When_PrivacyPolicyUrl_Exceeds_MaxLength()
        {
            var command = GetValidCommand();
            command.Dto.PrivacyPolicyUrl = "https://" + new string('a', 295) + ".com";
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.PrivacyPolicyUrl);
        }

        [Fact]
        public async Task Should_Have_Error_When_PrivacyPolicyUrl_Is_Invalid()
        {
            var command = GetValidCommand();
            command.Dto.PrivacyPolicyUrl = "not-a-url";
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.PrivacyPolicyUrl);
        }

        [Fact]
        public async Task Should_Have_Error_When_TechnicalContactEmail_Is_Invalid()
        {
            var command = GetValidCommand();
            command.Dto.TechnicalContactEmail = "not-an-email";
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.TechnicalContactEmail);
        }

        [Fact]
        public async Task Should_Have_Error_When_TechnicalContactEmail_Exceeds_MaxLength()
        {
            var command = GetValidCommand();
            command.Dto.TechnicalContactEmail = new string('a', 145) + "@ex.com";
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.TechnicalContactEmail);
        }

        [Fact]
        public async Task Should_Have_Error_When_TechnicalContactName_Exceeds_MaxLength()
        {
            var command = GetValidCommand();
            command.Dto.TechnicalContactName = new string('N', 101);
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.TechnicalContactName);
        }

        [Fact]
        public async Task Should_Have_Error_When_DataRetentionDescription_Exceeds_MaxLength()
        {
            var command = GetValidCommand();
            command.Dto.DataRetentionDescription = new string('D', 501);
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.DataRetentionDescription);
        }

        [Fact]
        public async Task Should_Have_Error_When_ExpectedRequestVolume_Is_Negative()
        {
            var command = GetValidCommand();
            command.Dto.ExpectedRequestVolume = -1;
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.ExpectedRequestVolume);
        }

        [Fact]
        public async Task Should_Have_Error_When_ClientId_Is_Empty()
        {
            var command = GetValidCommand();
            command.Dto.ClientId = Guid.Empty;
            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto.ClientId);
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_Optional_Fields_Are_Null_Or_Empty()
        {
            var command = new CreateApplicationFormCommand(
                new CreateApplicationFormDto
                {
                    ApplicationName = "Test App",
                    ApplicationDescription = "A test application.",
                    EmailAddress = "test@example.com",
                    OrganizationName = "Test Org",
                    CountryId = null,
                    ApplicationType = "Web",
                    RedirectUri = null,
                    Environment = "Sandbox",
                    ExpectedRequestVolume = null,
                    AcceptTerms = true,
                    PrivacyPolicyUrl = null,
                    DataRetentionDescription = null,
                    TechnicalContactName = null,
                    TechnicalContactEmail = null,
                    ClientId = Guid.NewGuid()
                }
            );
            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
