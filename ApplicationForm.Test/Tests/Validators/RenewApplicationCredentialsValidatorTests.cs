using Xunit;
using FluentValidation.TestHelper;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationCredentials;

namespace ApplicationForm.Test.Tests.Validators
{
    public class RenewApplicationCredentialsValidatorTests
    {
        private readonly RenewApplicationCredentialsValidator _validator = new RenewApplicationCredentialsValidator();

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var model = new RenewApplicationCredentialsCommand(Guid.Empty);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}
