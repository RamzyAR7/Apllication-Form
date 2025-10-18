using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Entities;
using Application_Form.Application.Feature.ApplicatioForm.Command.UpdateApplicationForm;
using Application_Form.Application.Feature.ApplicatioForm.Command.ChangeApplicationStatus;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationCredentials;
using System;
using System.Linq;

namespace ApplicationForm.Test.Tests
{
    [Collection("AppTestCollection")]
    public class GeneratedMassTests
    {
        private readonly TestFixture _fixture;
        public GeneratedMassTests(TestFixture fixture) { _fixture = fixture; }

        // Create success variations (10 cases)
        [Theory]
        [InlineData("AppA1","a1@a.com", true)]
        [InlineData("AppA2","a2@a.com", true)]
        [InlineData("AppA3","a3@a.com", true)]
        [InlineData("AppA4","a4@a.com", true)]
        [InlineData("AppA5","a5@a.com", true)]
        [InlineData("AppA6","a6@a.com", true)]
        [InlineData("AppA7","a7@a.com", true)]
        [InlineData("AppA8","a8@a.com", true)]
        [InlineData("AppA9","a9@a.com", true)]
        [InlineData("AppA10","a10@a.com", true)]
        public async Task Create_Various_Valid_Succeeds(string name, string email, bool accept)
        {
            var sp = _fixture.BuildServiceProvider("create_var_" + Guid.NewGuid());
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();
            var mapper = sp.GetRequiredService<AutoMapper.IMapper>();

            var client = new Client { Id = Guid.NewGuid(), Name = "C" };
            db.Clients.Add(client);
            await db.SaveChangesAsync();

            var dto = new CreateApplicationFormDto
            {
                ApplicationName = name,
                ApplicationDescription = "desc",
                EmailAddress = email,
                OrganizationName = "Org",
                ApplicationType = "Web",
                Environment = ApiEnvironment.Sandbox.ToString(),
                AcceptTerms = accept,
                ClientId = client.Id
            };

            var handler = new CreateApplicationFormHandler(repo, mapper, new ApplicationForm.Test.Tests.NoopLogger<CreateApplicationFormHandler>());
            var res = await handler.Handle(new CreateApplicationFormCommand(dto), System.Threading.CancellationToken.None);
            Assert.True(res.Success);
        }

        // Create invalid variations (10 cases) - expect validation failure
        [Theory]
        [InlineData("","bad@x.com", true)]
        [InlineData(null,"bad@x.com", true)]
        [InlineData("Name","not-an-email", true)]
        [InlineData("Name","ok@ok.com", false)]
        [InlineData("","", false)]
        [InlineData(null,null,false)]
        [InlineData("N1","", true)]
        [InlineData("N2","also-not-email", true)]
        public void Create_Invalid_ValidationFails(string name, string email, bool accept)
        {
            var dto = new CreateApplicationFormDto
            {
                ApplicationName = name,
                ApplicationDescription = "desc",
                EmailAddress = email,
                OrganizationName = "Org",
                ApplicationType = "Web",
                Environment = ApiEnvironment.Sandbox.ToString(),
                AcceptTerms = accept,
                ClientId = Guid.NewGuid()
            };
            var v = new CreateApplicationFormValidator();
            var r = v.Validate(new CreateApplicationFormCommand(dto));
            Assert.False(r.IsValid);
        }

        // Update variations: current status and expected outcome (6 cases)
        [Theory]
        [InlineData("Pending", true)]
        [InlineData("Approved", false)]
        [InlineData("Rejected", false)]
        [InlineData("Revoked", false)]
        [InlineData("SomeOther", false)]
        [InlineData("", false)]
        public async Task Update_StatusBased_Behavior(string currentStatus, bool expectSuccess)
        {
            var sp = _fixture.BuildServiceProvider("upd_var_" + Guid.NewGuid());
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();
            var mapper = sp.GetRequiredService<AutoMapper.IMapper>();

            var client = new Client { Id = Guid.NewGuid(), Name = "U" };
            db.Clients.Add(client);
            var ent = new Application_Form.Domain.Entities.ApplicationForm
            {
                ApplicationName = "ToUpd",
                ApplicationDescription = "D",
                EmailAddress = "u@u.com",
                OrganizationName = "Org",
                ApplicationType = "Web",
                Environment = ApiEnvironment.Sandbox.ToString(),
                ClientId = client.Id,
                ApprovalStatus = currentStatus,
                AcceptTerms = true
            };
            db.ApplicationForms.Add(ent);
            await db.SaveChangesAsync();

            var handler = new UpdateApplicationFormHandler(repo, mapper, new ApplicationForm.Test.Tests.NoopLogger<UpdateApplicationFormHandler>());
            var dto = new UpdateApplicationFormDto { ApplicationName = "N", ApplicationDescription = "D", EmailAddress = "e@e.com", OrganizationName = "O", ApplicationType = "Web", Environment = ApiEnvironment.Sandbox.ToString() };
            var res = await handler.Handle(new UpdateApplicationFormCommand(ent.Id, dto), System.Threading.CancellationToken.None);
            Assert.Equal(expectSuccess, res.Success);
        }

        // Change status transitions (8 cases): from,to,expected
        [Theory]
        [InlineData("Pending","Approved", true)]
        [InlineData("Pending","Rejected", true)]
        [InlineData("Approved","Revoked", true)]
        [InlineData("Rejected","Approved", false)]
        [InlineData("Approved","Approved", true)]
        [InlineData("Pending","Revoked", false)]
        [InlineData("Revoked","Approved", false)]
        [InlineData("Pending","Pending", true)]
        public async Task ChangeStatus_Transitions(string from, string to, bool expectedSuccess)
        {
            var sp = _fixture.BuildServiceProvider("chg_var_" + Guid.NewGuid());
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();
            var api = sp.GetRequiredService<Application_Form.Application.Services.IApiCredentialService>();

            var client = new Client { Id = Guid.NewGuid(), Name = "C" };
            db.Clients.Add(client);
            var ent = new Application_Form.Domain.Entities.ApplicationForm
            {
                ApplicationName = "S",
                ApplicationDescription = "D",
                EmailAddress = "s@s.com",
                OrganizationName = "Org",
                ApplicationType = "Web",
                Environment = ApiEnvironment.Sandbox.ToString(),
                ClientId = client.Id,
                ApprovalStatus = from,
                AcceptTerms = true
            };
            db.ApplicationForms.Add(ent);
            await db.SaveChangesAsync();

            var handler = new ChangeApplicationStatusHandler(repo, api, new ApplicationForm.Test.Tests.NoopLogger<ChangeApplicationStatusHandler>());
            var dto = new ChangeApplicationStatusDto { NewStatus = to };
            var res = await handler.Handle(new ChangeApplicationStatusCommand(ent.Id, dto), System.Threading.CancellationToken.None);
            if (from.Equals(to, StringComparison.OrdinalIgnoreCase))
                Assert.True(res.Success);
            else
                Assert.Equal(expectedSuccess, res.Success);
        }

        // Renew credential combos (8 cases): isActive, approvalStatus, isExpired => expected
        [Theory]
        [InlineData(true, "Approved", false, true)]
        [InlineData(false, "Approved", false, false)]
        [InlineData(true, "Pending", false, false)]
        [InlineData(true, "Approved", true, false)]
        [InlineData(false, "Rejected", false, false)]
        [InlineData(true, "Revoked", false, false)]
        public async Task Renew_Combinations(bool isActive, string approvalStatus, bool expired, bool expected)
        {
            var sp = _fixture.BuildServiceProvider("renew_var_" + Guid.NewGuid());
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();
            var api = sp.GetRequiredService<Application_Form.Application.Services.IApiCredentialService>();

            var client = new Client { Id = Guid.NewGuid(), Name = "R" };
            db.Clients.Add(client);
            var ent = new Application_Form.Domain.Entities.ApplicationForm
            {
                ApplicationName = "A",
                ApplicationDescription = "D",
                EmailAddress = "a@a.com",
                OrganizationName = "Org",
                ApplicationType = "Web",
                Environment = ApiEnvironment.Sandbox.ToString(),
                ClientId = client.Id,
                IsActive = isActive,
                ApprovalStatus = approvalStatus,
                AcceptTerms = true,
                ExpirationDate = expired ? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)) : (DateOnly?)null
            };
            db.ApplicationForms.Add(ent);
            await db.SaveChangesAsync();

            var handler = new RenewApplicationCredentialsHandler(repo, api, new ApplicationForm.Test.Tests.NoopLogger<RenewApplicationCredentialsHandler>());
            var res = await handler.Handle(new RenewApplicationCredentialsCommand(ent.Id), System.Threading.CancellationToken.None);
            Assert.Equal(expected, res.Success);
        }

        // Paging permutations (6 cases)
        [Theory]
        [InlineData(1,5)]
        [InlineData(1,1)]
        [InlineData(2,2)]
        [InlineData(10,5)]
        [InlineData(1,10)]
        [InlineData(3,3)]
        public async Task Paging_Various(int page, int pageSize)
        {
            var sp = _fixture.BuildServiceProvider("pg_" + Guid.NewGuid());
            var db = sp.GetRequiredService<Application_Form.Infrastructure.Data.ApplicationDbContext>();
            var repo = sp.GetRequiredService<Application_Form.Application.Interfaces.Repositories.IApplicationFormRepository>();
            var mapper = sp.GetRequiredService<AutoMapper.IMapper>();

            var client = new Client { Id = Guid.NewGuid(), Name = "P" };
            db.Clients.Add(client);
            for (int i = 0; i < 15; i++)
            {
                db.ApplicationForms.Add(new Application_Form.Domain.Entities.ApplicationForm
                {
                    ApplicationName = "P" + i,
                    ApplicationDescription = "D",
                    EmailAddress = $"p{i}@p.com",
                    OrganizationName = "Org",
                    ApplicationType = "Web",
                    Environment = ApiEnvironment.Sandbox.ToString(),
                    ClientId = client.Id,
                    ApprovalStatus = i % 2 == 0 ? Status.Approved.ToString() : Status.Pending.ToString(),
                    AcceptTerms = true
                });
            }
            await db.SaveChangesAsync();

            var handler = new Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged.GetApplicationsPagedHandler(repo, mapper, new ApplicationForm.Test.Tests.SinkLogger<Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged.GetApplicationsPagedHandler>());
            var res = await handler.Handle(new Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged.GetApplicationsPagedQuery { Page = page, PageSize = pageSize, SortBy = "ApplicationName", SortOrder = "asc", Status = "all" }, System.Threading.CancellationToken.None);
            Assert.True(res.Success);
        }
    }
}
