using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application_Form.Application.Feature.ApplicatioForm.Command.ChangeApplicationStatus;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Application.Services;
using Application_Form.Domain.Entities;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Common;
using ApplicationForm.Test.Tests;

using ApplicationFormEntity = Application_Form.Domain.Entities.ApplicationForm;

namespace ApplicationForm.Test.Tests.Handlers
{
    public class ChangeStatusHandlerTests
    {
        [Fact]
        public async Task ChangeStatus_WhenApplicationNotFound_ReturnsFailure()
        {
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ApplicationFormEntity)null);
            var apiMock = new Mock<IApiCredentialService>();
            var logger = new NoopLogger<ChangeApplicationStatusHandler>();
            var handler = new ChangeApplicationStatusHandler(repoMock.Object, apiMock.Object, logger);
            var cmd = new ChangeApplicationStatusCommand(Guid.NewGuid(), new ChangeApplicationStatusDto { NewStatus = Status.Approved.ToString() });

            var res = await handler.Handle(cmd, CancellationToken.None);

            Assert.False(res.Success);
        }

        [Fact]
        public async Task ChangeStatus_Approve_Succeeds_WhenAllowed()
        {
            var ent = new ApplicationFormEntity { Id = Guid.NewGuid(), ApprovalStatus = Status.Pending.ToString(), IsActive = false };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            repoMock.Setup(r => r.Update(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var apiMock = new Mock<IApiCredentialService>();
            apiMock.Setup(a => a.GenerateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ApiCredentials("k","i","s"));

            var logger = new NoopLogger<ChangeApplicationStatusHandler>();
            var handler = new ChangeApplicationStatusHandler(repoMock.Object, apiMock.Object, logger);

            var cmd = new ChangeApplicationStatusCommand(ent.Id, new ChangeApplicationStatusDto { NewStatus = Status.Approved.ToString(), ExpirationDate = DateOnly.FromDateTime(System.DateTime.UtcNow.AddDays(10)) });
            var res = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(res.Success);
            Assert.Equal(Status.Approved.ToString(), ent.ApprovalStatus);
            Assert.True(ent.IsActive);
            Assert.NotNull(ent.ApiKey);
        }
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
            var ent = new ApplicationFormEntity
            {
                Id = Guid.NewGuid(),
                ApplicationName = "S",
                ApplicationDescription = "D",
                EmailAddress = "s@s.com",
                OrganizationName = "Org",
                ApplicationType = "Web",
                Environment = ApiEnvironment.Sandbox.ToString(),
                ClientId = Guid.NewGuid(),
                ApprovalStatus = from,
                AcceptTerms = true
            };
            var repoMock = new Moq.Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            repoMock.Setup(r => r.Update(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var apiMock = new Moq.Mock<IApiCredentialService>();
            apiMock.Setup(a => a.GenerateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ApiCredentials("k","i","s"));
            var logger = new NoopLogger<ChangeApplicationStatusHandler>();
            var handler = new ChangeApplicationStatusHandler(repoMock.Object, apiMock.Object, logger);
            var dto = new ChangeApplicationStatusDto { NewStatus = to };
            var res = await handler.Handle(new ChangeApplicationStatusCommand(ent.Id, dto), CancellationToken.None);
            if (from.Equals(to, StringComparison.OrdinalIgnoreCase))
                Assert.True(res.Success);
            else
                Assert.Equal(expectedSuccess, res.Success);
        }
        [Fact]
        public async Task ChangeStatus_InvalidTransition_Fails()
        {
            var ent = new ApplicationFormEntity { Id = Guid.NewGuid(), ApprovalStatus = Status.Rejected.ToString(), IsActive = false };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            var apiMock = new Mock<IApiCredentialService>();
            var logger = new NoopLogger<ChangeApplicationStatusHandler>();
            var handler = new ChangeApplicationStatusHandler(repoMock.Object, apiMock.Object, logger);

            var cmd = new ChangeApplicationStatusCommand(ent.Id, new ChangeApplicationStatusDto { NewStatus = Status.Approved.ToString(), ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)) });
            var res = await handler.Handle(cmd, CancellationToken.None);

            Assert.False(res.Success);
        }
    }
}
