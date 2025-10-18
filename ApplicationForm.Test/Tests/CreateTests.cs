using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Entities;

namespace ApplicationForm.Test.Tests
{
    [Collection("AppTestCollection")]
    public class CreateTests
    {
        private readonly TestFixture _fixture;
        public CreateTests(TestFixture fixture) { _fixture = fixture; }

        [Fact]
        public async Task Create_Valid_Succeeds()
        {
            var sp = _fixture.BuildServiceProvider("create_valid");
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();

            var client = new Client { Id = System.Guid.NewGuid(), Name = "C" };
            db.Clients.Add(client);
            await db.SaveChangesAsync();

            var dto = new CreateApplicationFormDto { ApplicationName = "A", ApplicationDescription = "D", EmailAddress = "a@a.com", OrganizationName = "O", ApplicationType = "Web", Environment = ApiEnvironment.Sandbox.ToString(), AcceptTerms = true, ClientId = client.Id };
            var handler = new CreateApplicationFormHandler(repo, sp.GetRequiredService<AutoMapper.IMapper>(), new ApplicationForm.Test.Tests.NoopLogger<CreateApplicationFormHandler>());
            var res = await handler.Handle(new CreateApplicationFormCommand(dto), System.Threading.CancellationToken.None);
            Assert.True(res.Success);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Create_InvalidApplicationName_Fails(string name)
        {
            var dto = new CreateApplicationFormDto { ApplicationName = name, ApplicationDescription = "D", EmailAddress = "a@a.com", OrganizationName = "O", ApplicationType = "Web", Environment = ApiEnvironment.Sandbox.ToString(), AcceptTerms = true, ClientId = System.Guid.NewGuid() };
            var v = new CreateApplicationFormValidator();
            var r = v.Validate(new CreateApplicationFormCommand(dto));
            Assert.False(r.IsValid);
        }
    }
}
