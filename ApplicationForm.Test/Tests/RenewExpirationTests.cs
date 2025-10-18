using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application_Form.Domain.Entities;
using Application_Form.Domain.Constant;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationExpirationDate;

namespace ApplicationForm.Test.Tests
{
    [Collection("AppTestCollection")]
    public class RenewExpirationTests
    {
        private readonly TestFixture _fixture;
        public RenewExpirationTests(TestFixture fixture) { _fixture = fixture; }

        [Fact]
        public async Task RenewExpiration_Succeeds_For_Approved_Active_Application()
        {
            var sp = _fixture.BuildServiceProvider("renew_expiration_1");
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();

            var client = new Client { Id = System.Guid.NewGuid(), Name = "ClientX" };
            db.Clients.Add(client);

            var ent = new Application_Form.Domain.Entities.ApplicationForm
            {
                ApplicationName = "AppX",
                ApplicationDescription = "Desc",
                EmailAddress = "x@x.com",
                OrganizationName = "Org",
                ApplicationType = "Web",
                Environment = ApiEnvironment.Sandbox.ToString(),
                ClientId = client.Id,
                IsActive = true,
                ApprovalStatus = Status.Approved.ToString(),
                AcceptTerms = true,
                ExpirationDate = DateOnly.FromDateTime(System.DateTime.UtcNow.AddDays(7))
            };

            db.ApplicationForms.Add(ent);
            await db.SaveChangesAsync();

            var newExpiration = DateOnly.FromDateTime(System.DateTime.UtcNow.AddDays(60));

            var handler = new RenewApplicationExpirationDateHandler(repo, new ApplicationForm.Test.Tests.NoopLogger<RenewApplicationExpirationDateHandler>());
            var res = await handler.Handle(new RenewApplicationExpirationDateCommand(ent.Id, newExpiration), System.Threading.CancellationToken.None);

            Assert.True(res.Success);

            // verify persisted
            var updated = await repo.GetByIdAsync(ent.Id);
            Assert.NotNull(updated);
            Assert.Equal(newExpiration, updated.ExpirationDate);
        }
    }
}
