using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application_Form.Application.Feature.ApplicatioForm.Command.ChangeApplicationStatus;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Entities;

namespace ApplicationForm.Test.Tests
{
    [Collection("AppTestCollection")]
    public class ChangeStatusTests
    {
        private readonly TestFixture _fixture;
        public ChangeStatusTests(TestFixture fixture) { _fixture = fixture; }

        [Fact]
        public async Task Approve_Then_Revoke_And_InvalidTransition()
        {
            var sp = _fixture.BuildServiceProvider("chg_1");
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();
            var api = sp.GetRequiredService<Application_Form.Application.Services.IApiCredentialService>();

            var client = new Client { Id = System.Guid.NewGuid(), Name = "C" };
            db.Clients.Add(client);
            var ent = new Application_Form.Domain.Entities.ApplicationForm
            {
                ApplicationName = "S",
                ApplicationDescription = "D",
                EmailAddress = "s@s.com",
                OrganizationName = "Org",
                ApplicationType = "Web",
                Environment = Application_Form.Domain.Constant.ApiEnvironment.Sandbox.ToString(),
                ClientId = client.Id,
                ApprovalStatus = Status.Pending.ToString(),
                AcceptTerms = true
            };
            db.ApplicationForms.Add(ent);
            await db.SaveChangesAsync();

            var handler = new ChangeApplicationStatusHandler(repo, api, new ApplicationForm.Test.Tests.NoopLogger<ChangeApplicationStatusHandler>());
            var approveDto = new ChangeApplicationStatusDto { NewStatus = Status.Approved.ToString(), ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)) };
            var r = await handler.Handle(new ChangeApplicationStatusCommand(ent.Id, approveDto), System.Threading.CancellationToken.None);
            Assert.True(r.Success);

            var revokeDto = new ChangeApplicationStatusDto { NewStatus = Status.Revoked.ToString(), AdminNotes = "x" };
            var rr = await handler.Handle(new ChangeApplicationStatusCommand(ent.Id, revokeDto), System.Threading.CancellationToken.None);
            Assert.True(rr.Success);

            var pend = new Application_Form.Domain.Entities.ApplicationForm
            {
                ApplicationName = "P",
                ApplicationDescription = "D",
                EmailAddress = "p@p.com",
                OrganizationName = "Org",
                ApplicationType = "Web",
                Environment = Application_Form.Domain.Constant.ApiEnvironment.Sandbox.ToString(),
                ClientId = client.Id,
                ApprovalStatus = Status.Pending.ToString(),
                AcceptTerms = true
            };
            db.ApplicationForms.Add(pend);
            await db.SaveChangesAsync();
            var bad = await handler.Handle(new ChangeApplicationStatusCommand(pend.Id, new ChangeApplicationStatusDto { NewStatus = Status.Revoked.ToString() }), System.Threading.CancellationToken.None);
            Assert.False(bad.Success);
        }
    }
}
