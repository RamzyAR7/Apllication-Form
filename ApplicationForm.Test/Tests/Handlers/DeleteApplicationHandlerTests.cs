using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application_Form.Application.Feature.ApplicatioForm.Command.DeleteApplication;
using Application_Form.Application.Interfaces.Repositories;
using ApplicationForm.Test.Tests;

using ApplicationFormEntity = Application_Form.Domain.Entities.ApplicationForm;

namespace ApplicationForm.Test.Tests.Handlers
{
    public class DeleteApplicationHandlerTests
    {
        [Fact]
        public async Task Delete_WhenNotFound_ReturnsFailure()
        {
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ApplicationFormEntity)null);
            var logger = new NoopLogger<DeleteApplicationHandler>();
            var handler = new DeleteApplicationHandler(repoMock.Object, logger);

            var res = await handler.Handle(new DeleteApplicationCommand(Guid.NewGuid()), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task Delete_WhenAlreadyDeleted_ReturnsFailure()
        {
            var ent = new ApplicationFormEntity { Id = Guid.NewGuid(), IsDeleted = true };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            var logger = new NoopLogger<DeleteApplicationHandler>();
            var handler = new DeleteApplicationHandler(repoMock.Object, logger);

            var res = await handler.Handle(new DeleteApplicationCommand(ent.Id), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task Delete_Valid_Succeeds()
        {
            var ent = new ApplicationFormEntity { Id = Guid.NewGuid(), IsDeleted = false, IsActive = true };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            repoMock.Setup(r => r.Update(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var logger = new NoopLogger<DeleteApplicationHandler>();
            var handler = new DeleteApplicationHandler(repoMock.Object, logger);

            var res = await handler.Handle(new DeleteApplicationCommand(ent.Id), CancellationToken.None);
            Assert.True(res.Success);
            Assert.True(ent.IsDeleted);
            Assert.False(ent.IsActive);
        }
    }
}
