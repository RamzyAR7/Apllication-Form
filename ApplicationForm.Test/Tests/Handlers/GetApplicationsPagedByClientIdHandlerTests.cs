using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPagedByClientId;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Entities;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Constant;
using Application_Form.Domain.Common;
using Application_Form.Application.Models;
using AutoMapper;
using ApplicationForm.Test.Tests;

using ApplicationFormEntity = Application_Form.Domain.Entities.ApplicationForm;
using PaginatedListEntity = Application_Form.Application.Models.PaginatedList<Application_Form.Domain.Entities.ApplicationForm>;

namespace ApplicationForm.Test.Tests.Handlers
{
    public class GetApplicationsPagedByClientIdHandlerTests
    {
        [Fact]
        public async Task GetPagedByClient_WhenClientNotFound_ReturnsFailure()
        {
            var repoMock = new Mock<IApplicationFormRepository>();
            var clientRepoMock = new Mock<IClientRepository>();
            clientRepoMock.Setup(c => c.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((Client)null);
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<GetApplicationsPagedByClientIdHandler>();
            var handler = new GetApplicationsPagedByClientIdHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var res = await handler.Handle(new GetApplicationsPagedByClientIdQuery { ClientId = 1L }, CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task GetPagedByClient_Succeeds()
        {
            var pagedList = new PaginatedListEntity { Items = new[] { new ApplicationFormEntity { Id = 2L } }, TotalCount = 1, Page = 1, PageSize = 10 };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetPagedByClientIdAsync(It.IsAny<long>(), 1, 10, "CreatedAt", "desc", "all")).ReturnsAsync(pagedList);
            repoMock.Setup(r => r.Update(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var clientRepoMock = new Mock<IClientRepository>();
            clientRepoMock.Setup(c => c.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(new Client());
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<ApplicationFormListResponseDto>>(It.IsAny<IEnumerable<ApplicationFormEntity>>())).Returns(new[] { new ApplicationFormListResponseDto { Id = 3L } });
            var logger = new NoopLogger<GetApplicationsPagedByClientIdHandler>();
            var handler = new GetApplicationsPagedByClientIdHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var res = await handler.Handle(new GetApplicationsPagedByClientIdQuery { ClientId = 4L, Page = 1, PageSize = 10, SortBy = "CreatedAt", SortOrder = "desc", Status = "all" }, CancellationToken.None);
            Assert.True(res.Success);
        }
    }
}
