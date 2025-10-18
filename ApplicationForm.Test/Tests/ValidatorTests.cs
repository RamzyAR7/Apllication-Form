using Xunit;
using Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;

namespace ApplicationForm.Test.Tests
{
    [Collection("AppTestCollection")]
    public class ValidatorTests
    {
        private readonly TestFixture _fixture;
        public ValidatorTests(TestFixture fixture) { _fixture = fixture; }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CreateValidator_Fails_On_Bad_Name(string name)
        {
            var dto = new CreateApplicationFormDto { ApplicationName = name, ApplicationDescription = "D", EmailAddress = "a@a.com", OrganizationName = "O", ApplicationType = "Web", Environment = ApiEnvironment.Sandbox.ToString(), AcceptTerms = true, ClientId = System.Guid.NewGuid() };
            var v = new CreateApplicationFormValidator();
            var r = v.Validate(new CreateApplicationFormCommand(dto));
            Assert.False(r.IsValid);
        }
    }
}
