using Xunit;
using FluentValidation.TestHelper;
using Application_Form.Application.Feature.ApplicatioForm.Command.UpdateApplicationForm;
using Application_Form.Application.DTOs;

namespace ApplicationForm.Test.Tests.Validators
{
    public class UpdateApplicationFormValidatorTests
    {
        private readonly UpdateApplicationFormValidator _validator = new UpdateApplicationFormValidator();

        [Fact]
        public async Task Should_Have_Error_When_Id_Is_Empty()
        {
            var model = new UpdateApplicationFormCommand(0L, new UpdateApplicationFormDto());
            var result = await _validator.TestValidateAsync(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}
