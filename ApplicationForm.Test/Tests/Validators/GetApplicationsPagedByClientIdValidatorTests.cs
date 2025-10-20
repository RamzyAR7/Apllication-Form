using Xunit;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPagedByClientId;
using FluentValidation.TestHelper;

namespace ApplicationForm.Test.Tests.Validators
{
    public class GetApplicationsPagedByClientIdValidatorTests
    {
        private readonly GetApplicationsPagedByClientIdValidator _validator = new GetApplicationsPagedByClientIdValidator();

        [Fact]
        public void Should_Have_Error_When_ClientId_Is_Empty()
        {
            var model = new GetApplicationsPagedByClientIdQuery { ClientId = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ClientId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ClientId_Is_Valid()
        {
            var model = new GetApplicationsPagedByClientIdQuery { ClientId = Guid.NewGuid() };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ClientId);
        }
    }
}
