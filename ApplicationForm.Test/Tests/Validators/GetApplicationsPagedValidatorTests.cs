using Xunit;
using FluentValidation.TestHelper;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged;

namespace ApplicationForm.Test.Tests.Validators
{
    public class GetApplicationsPagedValidatorTests
    {
        private readonly GetApplicationsPagedValidator _validator = new GetApplicationsPagedValidator();

        [Fact]
        public void Should_Have_Error_When_Page_Is_Less_Than_One()
        {
            var model = new GetApplicationsPagedQuery { Page = 0, PageSize = 10, SortOrder = "asc", Status = "All", SortBy = "CreatedAt" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Page);
        }
    }
}
