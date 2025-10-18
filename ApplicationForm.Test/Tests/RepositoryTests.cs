using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application_Form.Infrastructure.Data;
using Application_Form.Domain.Entities;

namespace ApplicationForm.Test.Tests
{
    [Collection("AppTestCollection")]
    public class RepositoryTests
    {
        private readonly TestFixture _fixture;
        public RepositoryTests(TestFixture fixture) { _fixture = fixture; }

        [Fact]
        public async Task Add_Get_Paging()
        {
            var sp = _fixture.BuildServiceProvider("repo_1");
            var db = sp.GetRequiredService<ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();

            var client = new Client { Id = System.Guid.NewGuid(), Name = "R" };
            db.Clients.Add(client);
            await db.SaveChangesAsync();

            var ent = new Application_Form.Domain.Entities.ApplicationForm { ApplicationName = "Rp", ApplicationDescription = "D", EmailAddress = "r@r.com", OrganizationName = "Org", ApplicationType = "Web", Environment = Application_Form.Domain.Constant.ApiEnvironment.Sandbox.ToString(), ClientId = client.Id, AcceptTerms = true };
            await repo.AddAsync(ent);
            await repo.SaveChangesAsync();

            var f = await repo.GetByIdAsync(ent.Id);
            Assert.NotNull(f);

            var paged = await repo.GetPagedApplicationsAsync(1, 10, "ApplicationName", "asc", "all");
            Assert.True(paged.TotalCount >= 1);
        }
    }
}
