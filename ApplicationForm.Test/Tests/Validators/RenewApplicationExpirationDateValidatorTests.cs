using Xunit;
using FluentValidation.TestHelper;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationExpirationDate;
using System;

namespace ApplicationForm.Test.Tests.Validators
{
    public class RenewApplicationExpirationDateValidatorTests
    {
        private readonly RenewApplicationExpirationDateValidator _validator = new RenewApplicationExpirationDateValidator();

        [Fact]
        public void Should_Have_Error_When_ApplicationId_Is_Empty()
        {
            var model = new RenewApplicationExpirationDateCommand(Guid.Empty, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ApplicationId);
        }
    }
}
