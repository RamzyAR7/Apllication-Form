using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationCredentials;
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
    public class RenewApplicationCredentialsHandlerTests
    {
        [Fact]
        public async Task RenewCredentials_WhenNotFound_ReturnsFailure()
        {
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((ApplicationFormEntity)null);
            var apiMock = new Mock<IApiCredentialService>();
            var logger = new NoopLogger<RenewApplicationCredentialsHandler>();
            var handler = new RenewApplicationCredentialsHandler(repoMock.Object, apiMock.Object, logger);

            var res = await handler.Handle(new RenewApplicationCredentialsCommand(1L), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task RenewCredentials_WhenNotActive_ReturnsFailure()
        {
            var ent = new ApplicationFormEntity { Id = 2L, IsActive = false, ApprovalStatus = Status.Approved.ToString() };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            var apiMock = new Mock<IApiCredentialService>();
            var logger = new NoopLogger<RenewApplicationCredentialsHandler>();
            var handler = new RenewApplicationCredentialsHandler(repoMock.Object, apiMock.Object, logger);

            var res = await handler.Handle(new RenewApplicationCredentialsCommand(ent.Id), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task RenewCredentials_WhenNotApproved_ReturnsFailure()
        {
            var ent = new ApplicationFormEntity { Id = 3L, IsActive = true, ApprovalStatus = Status.Pending.ToString() };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            var apiMock = new Mock<IApiCredentialService>();
            var logger = new NoopLogger<RenewApplicationCredentialsHandler>();
            var handler = new RenewApplicationCredentialsHandler(repoMock.Object, apiMock.Object, logger);

            var res = await handler.Handle(new RenewApplicationCredentialsCommand(ent.Id), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task RenewCredentials_WhenExpired_ReturnsFailure()
        {
            var ent = new ApplicationFormEntity { Id = 4L, IsActive = true, ApprovalStatus = Status.Approved.ToString(), ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)) };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            var apiMock = new Mock<IApiCredentialService>();
            var logger = new NoopLogger<RenewApplicationCredentialsHandler>();
            var handler = new RenewApplicationCredentialsHandler(repoMock.Object, apiMock.Object, logger);

            var res = await handler.Handle(new RenewApplicationCredentialsCommand(ent.Id), CancellationToken.None);
            Assert.False(res.Success);
        }
        [Theory]
        [InlineData(true, "Approved", false, true)]
        [InlineData(false, "Approved", false, false)]
        [InlineData(true, "Pending", false, false)]
        [InlineData(true, "Approved", true, false)]
        [InlineData(false, "Rejected", false, false)]
        [InlineData(true, "Revoked", false, false)]
        public async Task Renew_Combinations(bool isActive, string approvalStatus, bool expired, bool expected)
        {
            var ent = new ApplicationFormEntity
            {
                Id = 5L,
                ApplicationName = "A",
                ApplicationDescription = "D",
                EmailAddress = "a@a.com",
                OrganizationName = "Org",
                ApplicationType = "Web",
                Environment = ApiEnvironment.Sandbox.ToString(),
                ClientId = 6L,
                IsActive = isActive,
                ApprovalStatus = approvalStatus,
                AcceptTerms = true,
                ExpirationDate = expired ? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)) : (DateOnly?)null
            };
            var repoMock = new Moq.Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            repoMock.Setup(r => r.Update(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var apiMock = new Moq.Mock<IApiCredentialService>();
            apiMock.Setup(a => a.GenerateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ApiCredentials("k","i","s"));
            var logger = new NoopLogger<RenewApplicationCredentialsHandler>();
            var handler = new RenewApplicationCredentialsHandler(repoMock.Object, apiMock.Object, logger);
            var res = await handler.Handle(new RenewApplicationCredentialsCommand(ent.Id), CancellationToken.None);
            Assert.Equal(expected, res.Success);
        }
        [Fact]
        public async Task RenewCredentials_Valid_Succeeds()
        { 
            var ent = new ApplicationFormEntity { Id = 7L, IsActive = true, ApprovalStatus = Status.Approved.ToString(), ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)) };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            repoMock.Setup(r => r.Update(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var apiMock = new Mock<IApiCredentialService>();
            apiMock.Setup(a => a.GenerateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new ApiCredentials("newkey", "newid", "newsecret"));
            var logger = new NoopLogger<RenewApplicationCredentialsHandler>();
            var handler = new RenewApplicationCredentialsHandler(repoMock.Object, apiMock.Object, logger);

            var res = await handler.Handle(new RenewApplicationCredentialsCommand(ent.Id), CancellationToken.None);
            Assert.True(res.Success);
            Assert.Equal("newkey", ent.ApiKey);
        }
    }
}
