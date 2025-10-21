using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application_Form.Application.Feature.ApplicatioForm.Command.UpdateApplicationForm;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Entities;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Common;
using AutoMapper;
using Application_Form.Application.Models;
using ApplicationForm.Test.Tests;

using ApplicationFormEntity = Application_Form.Domain.Entities.ApplicationForm;
using PaginatedListEntity = Application_Form.Application.Models.PaginatedList<Application_Form.Domain.Entities.ApplicationForm>;

namespace ApplicationForm.Test.Tests.Handlers
{
    public class UpdateApplicationFormHandlerTests
    {
        [Fact]
        public async Task Update_WhenNotFound_ReturnsFailure()
        {
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((ApplicationFormEntity)null);
            var clientRepoMock = new Mock<IClientRepository>();
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<UpdateApplicationFormHandler>();
            var handler = new UpdateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var dto = new UpdateApplicationFormDto { ClientId = 1L };
            var res = await handler.Handle(new UpdateApplicationFormCommand(1L, dto), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task Update_WhenClientNotExists_ReturnsFailure()
        {
            var ent = new ApplicationFormEntity { Id = 2L, ApprovalStatus = Status.Pending.ToString() };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            var clientRepoMock = new Mock<IClientRepository>();
            clientRepoMock.Setup(c => c.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Client)null);
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<UpdateApplicationFormHandler>();
            var handler = new UpdateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var dto = new UpdateApplicationFormDto { ClientId = 3L };
            var res = await handler.Handle(new UpdateApplicationFormCommand(ent.Id, dto), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task Update_WhenNotPending_ReturnsFailure()
        {
            var ent = new ApplicationFormEntity { Id = 4L, ApprovalStatus = Status.Approved.ToString() };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            var clientRepoMock = new Mock<IClientRepository>();
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<UpdateApplicationFormHandler>();
            var handler = new UpdateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var dto = new UpdateApplicationFormDto { ClientId = 5L };
            var res = await handler.Handle(new UpdateApplicationFormCommand(ent.Id, dto), CancellationToken.None);
            Assert.False(res.Success);
        }
        [Theory]
        [InlineData("Pending", true)]
        [InlineData("Approved", false)]
        [InlineData("Rejected", false)]
        [InlineData("Revoked", false)]
        [InlineData("SomeOther", false)]
        [InlineData("", false)]
        public async Task Update_StatusBased_Behavior(string currentStatus, bool expectSuccess)
        {
            var ent = new ApplicationFormEntity { Id = 6L, ApplicationName = "ToUpd", ApprovalStatus = currentStatus, ClientId = 7L };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            repoMock.Setup(r => r.GetPagedByClientIdAsync(It.IsAny<long>(), 1, 1000, "CreatedAt", "asc", "all")).ReturnsAsync(new PaginatedListEntity { Items = new ApplicationFormEntity[0] });
            repoMock.Setup(r => r.Update(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var clientRepoMock = new Mock<IClientRepository>();
            clientRepoMock.Setup(c => c.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(new Client());
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<UpdateApplicationFormHandler>();
            var handler = new UpdateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);
            var dto = new UpdateApplicationFormDto { ApplicationName = "N", ApplicationDescription = "D", EmailAddress = "e@example.com", OrganizationName = "O", ApplicationType = "Web", Environment = ApiEnvironment.Sandbox.ToString(), ClientId = ent.ClientId };
            var res = await handler.Handle(new UpdateApplicationFormCommand(ent.Id, dto), CancellationToken.None);
            Assert.Equal(expectSuccess, res.Success);
        }
        [Fact]
        public async Task Update_WhenClientMismatch_ReturnsFailure()
        {
            var ent = new ApplicationFormEntity { Id = 8L, ApprovalStatus = Status.Pending.ToString(), ClientId = 9L };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            var clientRepoMock = new Mock<IClientRepository>();
            clientRepoMock.Setup(c => c.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(new Client());
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<UpdateApplicationFormHandler>();
            var handler = new UpdateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var dto = new UpdateApplicationFormDto { ClientId = 10L };
            var res = await handler.Handle(new UpdateApplicationFormCommand(ent.Id, dto), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task Update_WhenNameExists_ReturnsFailure()
        {
            var ent = new ApplicationFormEntity { Id = 11L, ApprovalStatus = Status.Pending.ToString(), ClientId = 12L };
            var existing = new ApplicationFormEntity { Id = 13L, ApplicationName = "Existing" };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            repoMock.Setup(r => r.GetPagedByClientIdAsync(It.IsAny<long>(), 1, 1000, "CreatedAt", "asc", "all")).ReturnsAsync(new PaginatedListEntity { Items = new[] { existing } });
            var clientRepoMock = new Mock<IClientRepository>();
            clientRepoMock.Setup(c => c.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(new Client());
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<UpdateApplicationFormHandler>();
            var handler = new UpdateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var dto = new UpdateApplicationFormDto { ClientId = ent.ClientId, ApplicationName = "Existing" };
            var res = await handler.Handle(new UpdateApplicationFormCommand(ent.Id, dto), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task Update_Valid_Succeeds()
        {
            var ent = new ApplicationFormEntity { Id = 14L, ApprovalStatus = Status.Pending.ToString(), ClientId = 15L };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            repoMock.Setup(r => r.GetPagedByClientIdAsync(It.IsAny<long>(), 1, 1000, "CreatedAt", "asc", "all")).ReturnsAsync(new PaginatedListEntity { Items = new ApplicationFormEntity[0] });
            repoMock.Setup(r => r.Update(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var clientRepoMock = new Mock<IClientRepository>();
            clientRepoMock.Setup(c => c.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(new Client());
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<UpdateApplicationFormHandler>();
            var handler = new UpdateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var dto = new UpdateApplicationFormDto { ClientId = ent.ClientId, ApplicationName = "NewName" };
            var res = await handler.Handle(new UpdateApplicationFormCommand(ent.Id, dto), CancellationToken.None);
            Assert.True(res.Success);
        }
    }
}
