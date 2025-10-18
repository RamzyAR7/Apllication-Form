using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPagedByClientId;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationById;
using Application_Form.Domain.Entities;
using Application_Form.Domain.Constant;
using System.Linq;

namespace ApplicationForm.Test.Tests
{
    [Collection("AppTestCollection")]
    public class QueryTests
    {
        private readonly TestFixture _fixture;
        public QueryTests(TestFixture fixture) { _fixture = fixture; }

        [Fact]
        public async Task Paged_And_ByClient_And_ById()
        {
            var sp = _fixture.BuildServiceProvider("qry_1");
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();
            var mapper = sp.GetRequiredService<AutoMapper.IMapper>();

            var client = new Client { Id = System.Guid.NewGuid(), Name = "Q" };
            db.Clients.Add(client);
            for (int i = 0; i < 5; i++)
                db.ApplicationForms.Add(new Application_Form.Domain.Entities.ApplicationForm
                {
                    ApplicationName = "Q" + i,
                    ApplicationDescription = "D",
                    EmailAddress = $"q{i}@q.com",
                    OrganizationName = "Org",
                    ApplicationType = "Web",
                    Environment = Application_Form.Domain.Constant.ApiEnvironment.Sandbox.ToString(),
                    ClientId = client.Id,
                    ApprovalStatus = i % 2 == 0 ? Status.Approved.ToString() : Status.Pending.ToString(),
                    AcceptTerms = true
                });
            await db.SaveChangesAsync();

            var pagedHandler = new GetApplicationsPagedHandler(repo, mapper, new ApplicationForm.Test.Tests.SinkLogger<GetApplicationsPagedHandler>());
            var res = await pagedHandler.Handle(new GetApplicationsPagedQuery { Page = 1, PageSize = 2, SortBy = "ApplicationName", SortOrder = "asc", Status = "all" }, System.Threading.CancellationToken.None);
            Assert.True(res.Success);

            var byClient = new GetApplicationsPagedByClientIdHandler(repo, mapper, new ApplicationForm.Test.Tests.SinkLogger<GetApplicationsPagedByClientIdHandler>());
            var r2 = await byClient.Handle(new GetApplicationsPagedByClientIdQuery { ClientId = client.Id, Page = 1, PageSize = 10, SortBy = "ApplicationName", SortOrder = "asc", Status = "all" }, System.Threading.CancellationToken.None);
            Assert.True(r2.Success);

            var any = db.ApplicationForms.First();
            var byId = new GetApplicationByIdHandler(repo, mapper, new ApplicationForm.Test.Tests.SinkLogger<GetApplicationByIdHandler>());
            var r3 = await byId.Handle(new GetApplicationByIdQuery(any.Id), System.Threading.CancellationToken.None);
            Assert.True(r3.Success);
        }
    }
}
