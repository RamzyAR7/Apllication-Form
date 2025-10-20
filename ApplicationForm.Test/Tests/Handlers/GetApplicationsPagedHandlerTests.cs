using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged;
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
    public class GetApplicationsPagedHandlerTests
    {
        [Fact]
        public async Task GetPaged_Succeeds()
        {
            var pagedList = new PaginatedListEntity { Items = new[] { new ApplicationFormEntity { Id = Guid.NewGuid() } }, TotalCount = 1, Page = 1, PageSize = 10 };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetPagedApplicationsAsync(1, 10, "CreatedAt", "desc", "all")).ReturnsAsync(pagedList);
            repoMock.Setup(r => r.Update(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<ApplicationFormListResponseDto>>(It.IsAny<IEnumerable<ApplicationFormEntity>>())).Returns(new[] { new ApplicationFormListResponseDto { Id = Guid.NewGuid() } });
            var logger = new NoopLogger<GetApplicationsPagedHandler>();
            var handler = new GetApplicationsPagedHandler(repoMock.Object, mapperMock.Object, logger);

            var res = await handler.Handle(new GetApplicationsPagedQuery { Page = 1, PageSize = 10, SortBy = "CreatedAt", SortOrder = "desc", Status = "all" }, CancellationToken.None);
            Assert.True(res.Success);
        }
        [Theory]
        [InlineData(1,5)]
        [InlineData(1,1)]
        [InlineData(2,2)]
        [InlineData(10,5)]
        [InlineData(1,10)]
        [InlineData(3,3)]
        public async Task Paging_Various(int page, int pageSize)
        {
            var pagedList = new PaginatedListEntity
            {
                Items = new[] { new ApplicationFormEntity { Id = Guid.NewGuid() } },
                TotalCount = 15,
                Page = page,
                PageSize = pageSize
            };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetPagedApplicationsAsync(page, pageSize, "ApplicationName", "asc", "all")).ReturnsAsync(pagedList);
            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<IEnumerable<ApplicationFormListResponseDto>>(It.IsAny<IEnumerable<ApplicationFormEntity>>())).Returns(new[] { new ApplicationFormListResponseDto { Id = Guid.NewGuid() } });
            var logger = new NoopLogger<GetApplicationsPagedHandler>();
            var handler = new GetApplicationsPagedHandler(repoMock.Object, mapperMock.Object, logger);
            var res = await handler.Handle(new GetApplicationsPagedQuery { Page = page, PageSize = pageSize, SortBy = "ApplicationName", SortOrder = "asc", Status = "all" }, CancellationToken.None);
            Assert.True(res.Success);
        }
    }
}
