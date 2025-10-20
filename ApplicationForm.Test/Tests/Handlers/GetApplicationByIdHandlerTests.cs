using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationById;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Entities;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Common;
using AutoMapper;
using ApplicationForm.Test.Tests;

using ApplicationFormEntity = Application_Form.Domain.Entities.ApplicationForm;

namespace ApplicationForm.Test.Tests.Handlers
{
    public class GetApplicationByIdHandlerTests
    {
        [Fact]
        public async Task GetById_WhenNotFound_ReturnsFailure()
        {
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ApplicationFormEntity)null);
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<GetApplicationByIdHandler>();
            var handler = new GetApplicationByIdHandler(repoMock.Object, mapperMock.Object, logger);

            var res = await handler.Handle(new GetApplicationByIdQuery(Guid.NewGuid()), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task GetById_Valid_Succeeds()
        {
            var ent = new ApplicationFormEntity { Id = Guid.NewGuid(), ApplicationName = "Test" };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByIdAsync(ent.Id)).ReturnsAsync(ent);
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<ApplicationFormResponseDto>(ent)).Returns(new ApplicationFormResponseDto { Id = ent.Id });
            var logger = new NoopLogger<GetApplicationByIdHandler>();
            var handler = new GetApplicationByIdHandler(repoMock.Object, mapperMock.Object, logger);

            var res = await handler.Handle(new GetApplicationByIdQuery(ent.Id), CancellationToken.None);
            Assert.True(res.Success);
        }
    }
}
