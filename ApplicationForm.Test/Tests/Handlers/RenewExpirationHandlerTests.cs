using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationExpirationDate;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Entities;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Common;
using ApplicationForm.Test.Tests;

using ApplicationFormEntity = Application_Form.Domain.Entities.ApplicationForm;

namespace ApplicationForm.Test.Tests.Handlers
{
    public class RenewExpirationHandlerTests
    {
        [Fact]
        public async Task RenewExpiration_WhenNotFound_ReturnsFailure()
        {
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ApplicationFormEntity)null);
            var logger = new NoopLogger<RenewApplicationExpirationDateHandler>();
            var handler = new RenewApplicationExpirationDateHandler(repoMock.Object, logger);

            var res = await handler.Handle(new RenewApplicationExpirationDateCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10))), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task RenewExpiration_WhenInvalidDate_Fails()
        {
            var ent = new ApplicationFormEntity { Id = System.Guid.NewGuid(), ApprovalStatus = Status.Approved.ToString(), ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)) };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            var logger = new NoopLogger<RenewApplicationExpirationDateHandler>();
            var handler = new RenewApplicationExpirationDateHandler(repoMock.Object, logger);

            var res = await handler.Handle(new RenewApplicationExpirationDateCommand(ent.Id, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task RenewExpiration_ValidFutureDate_Succeeds()
        {
            var ent = new ApplicationFormEntity { Id = System.Guid.NewGuid(), ApprovalStatus = Status.Approved.ToString(), ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)), IsActive = false };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            repoMock.Setup(r => r.Update(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var logger = new NoopLogger<RenewApplicationExpirationDateHandler>();
            var handler = new RenewApplicationExpirationDateHandler(repoMock.Object, logger);

            var newDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
            var res = await handler.Handle(new RenewApplicationExpirationDateCommand(ent.Id, newDate), CancellationToken.None);

            Assert.True(res.Success);
            Assert.Equal(newDate, ent.ExpirationDate);
            Assert.Equal(Status.Approved.ToString(), ent.ApprovalStatus);
            Assert.True(ent.IsActive);
        }

        [Fact]
        public void Validator_Fails_When_ApplicationId_Is_Empty()
        {
            var cmd = new RenewApplicationExpirationDateCommand(System.Guid.Empty, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));
            var validator = new RenewApplicationExpirationDateValidator();

            var result = validator.Validate(cmd);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ApplicationId");
        }

        [Fact]
        public void Validator_Fails_When_NewExpirationDate_Is_Not_Future()
        {
            var cmd = new RenewApplicationExpirationDateCommand(System.Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)));
            var validator = new RenewApplicationExpirationDateValidator();

            var result = validator.Validate(cmd);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "NewExpirationDate");
        }

        [Fact]
        public void Validator_Succeeds_For_Valid_Command()
        {
            var cmd = new RenewApplicationExpirationDateCommand(System.Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));
            var validator = new RenewApplicationExpirationDateValidator();

            var result = validator.Validate(cmd);

            Assert.True(result.IsValid);
        }
    }
}
