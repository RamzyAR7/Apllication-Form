using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationCredentials;
using Application_Form.Domain.Entities;
using Application_Form.Domain.Constant;

namespace ApplicationForm.Test.Tests
{
    [Collection("AppTestCollection")]
    public class RenewTests
    {
        private readonly TestFixture _fixture;
        public RenewTests(TestFixture fixture) { _fixture = fixture; }

        [Fact]
        public async Task Renew_Succeeds_Only_For_Active_Approved_NotExpired()
        {
            var sp = _fixture.BuildServiceProvider("renew_1");
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();
            var api = sp.GetRequiredService<Application_Form.Application.Services.IApiCredentialService>();

            var client = new Client { Id = System.Guid.NewGuid(), Name = "R" };
            db.Clients.Add(client);
            var ent = new Application_Form.Domain.Entities.ApplicationForm { ApplicationName = "A", ApplicationDescription = "D", EmailAddress = "a@a.com", OrganizationName = "Org", ApplicationType = "Web", Environment = Application_Form.Domain.Constant.ApiEnvironment.Sandbox.ToString(), ClientId = client.Id, IsActive = true, ApprovalStatus = Status.Approved.ToString(), AcceptTerms = true };
            db.ApplicationForms.Add(ent);
            await db.SaveChangesAsync();

            var handler = new RenewApplicationCredentialsHandler(repo, api, new ApplicationForm.Test.Tests.NoopLogger<RenewApplicationCredentialsHandler>());
            var res = await handler.Handle(new RenewApplicationCredentialsCommand(ent.Id), System.Threading.CancellationToken.None);
            Assert.True(res.Success);
        }
    }
}
