using Xunit;
using FluentValidation.TestHelper;
using Application_Form.Application.Feature.ApplicatioForm.Command.DeleteApplication;

namespace ApplicationForm.Test.Tests.Validators
{
    public class DeleteApplicationValidatorTests
    {
        private readonly DeleteApplicationValidator _validator = new DeleteApplicationValidator();

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var model = new DeleteApplicationCommand(0L);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}
