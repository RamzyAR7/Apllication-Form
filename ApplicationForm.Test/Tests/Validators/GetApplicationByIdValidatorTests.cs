using Xunit;
using FluentValidation.TestHelper;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationById;

namespace ApplicationForm.Test.Tests.Validators
{
    public class GetApplicationByIdValidatorTests
    {
        private readonly GetApplicationByIdValidator _validator = new GetApplicationByIdValidator();

        [Fact]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var model = new GetApplicationByIdQuery(Guid.Empty);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}
