using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application_Form.Application.Feature.ApplicatioForm.Command.UpdateApplicationForm;
using Application_Form.Application.Feature.ApplicatioForm.Command.DeleteApplication;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Entities;

namespace ApplicationForm.Test.Tests
{
    [Collection("AppTestCollection")]
    public class UpdateDeleteTests
    {
        private readonly TestFixture _fixture;
        public UpdateDeleteTests(TestFixture fixture) { _fixture = fixture; }

        [Fact]
        public async Task Update_Pending_Succeeds_And_Update_NonExisting_Fails()
        {
            var sp = _fixture.BuildServiceProvider("upd_1");
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();
            var mapper = sp.GetRequiredService<AutoMapper.IMapper>();

            var client = new Client { Id = System.Guid.NewGuid(), Name = "U" };
            db.Clients.Add(client);
            var ent = new Application_Form.Domain.Entities.ApplicationForm { ApplicationName = "ToUpd", ApplicationDescription = "D", EmailAddress = "u@u.com", OrganizationName = "Org", ApplicationType = "Web", Environment = Application_Form.Domain.Constant.ApiEnvironment.Sandbox.ToString(), ClientId = client.Id, ApprovalStatus = Status.Pending.ToString(), AcceptTerms = true };
            db.ApplicationForms.Add(ent);
            await db.SaveChangesAsync();

            var handler = new UpdateApplicationFormHandler(repo, mapper, new ApplicationForm.Test.Tests.NoopLogger<UpdateApplicationFormHandler>());
            var dto = new UpdateApplicationFormDto { ApplicationName = "N", ApplicationDescription = "D", EmailAddress = "e@e.com", OrganizationName = "O", ApplicationType = "Web", Environment = ApiEnvironment.Sandbox.ToString() };
            var res = await handler.Handle(new UpdateApplicationFormCommand(ent.Id, dto), System.Threading.CancellationToken.None);
            Assert.True(res.Success);

            var res2 = await handler.Handle(new UpdateApplicationFormCommand(System.Guid.NewGuid(), dto), System.Threading.CancellationToken.None);
            Assert.False(res2.Success);
        }

        [Fact]
        public async Task Delete_Marks_Deleted()
        {
            var sp = _fixture.BuildServiceProvider("del_1");
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();

            var client = new Client { Id = System.Guid.NewGuid(), Name = "D" };
            db.Clients.Add(client);
            var ent = new Application_Form.Domain.Entities.ApplicationForm { ApplicationName = "ToDel", ApplicationDescription = "D", EmailAddress = "d@d.com", OrganizationName = "Org", ApplicationType = "Web", Environment = Application_Form.Domain.Constant.ApiEnvironment.Sandbox.ToString(), ClientId = client.Id, IsActive = true, AcceptTerms = true };
            db.ApplicationForms.Add(ent);
            await db.SaveChangesAsync();

            var handler = new DeleteApplicationHandler(repo, new ApplicationForm.Test.Tests.NoopLogger<DeleteApplicationHandler>());
            var r = await handler.Handle(new DeleteApplicationCommand(ent.Id), System.Threading.CancellationToken.None);
            Assert.True(r.Success);

            var fetched = await repo.GetByIdAsync(ent.Id);
            Assert.Null(fetched);
        }
    }
}
