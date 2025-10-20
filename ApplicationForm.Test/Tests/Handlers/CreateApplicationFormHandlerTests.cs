using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm;
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
    public class CreateApplicationFormHandlerTests
    {
        [Fact]
        public async Task Create_WhenClientNotFound_ReturnsFailure()
        {
            var repoMock = new Mock<IApplicationFormRepository>();
            var clientRepoMock = new Mock<IClientRepository>();
            clientRepoMock.Setup(c => c.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client)null);
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<CreateApplicationFormHandler>();
            var handler = new CreateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var dto = new CreateApplicationFormDto { ClientId = Guid.NewGuid(), ApplicationName = "Test" };
            var res = await handler.Handle(new CreateApplicationFormCommand(dto), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task Create_WhenApplicationNameExists_ReturnsFailure()
        {
            var client = new Client { Id = Guid.NewGuid() };
            var existingApp = new ApplicationFormEntity { Id = Guid.NewGuid(), ApplicationName = "Test", ClientId = client.Id };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByNameAndClientIdAsync("Test", client.Id)).ReturnsAsync(existingApp);
            var clientRepoMock = new Mock<IClientRepository>();
            clientRepoMock.Setup(c => c.GetByIdAsync(client.Id)).ReturnsAsync(client);
            var mapperMock = new Mock<IMapper>();
            var logger = new NoopLogger<CreateApplicationFormHandler>();
            var handler = new CreateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var dto = new CreateApplicationFormDto { ClientId = client.Id, ApplicationName = "Test" };
            var res = await handler.Handle(new CreateApplicationFormCommand(dto), CancellationToken.None);
            Assert.False(res.Success);
        }

        [Fact]
        public async Task Create_Valid_Succeeds()
        {
            var client = new Client { Id = Guid.NewGuid() };
            var repoMock = new Mock<IApplicationFormRepository>();
            repoMock.Setup(r => r.GetByNameAndClientIdAsync(It.IsAny<string>(), client.Id)).ReturnsAsync((ApplicationFormEntity)null);
            repoMock.Setup(r => r.AddAsync(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            var clientRepoMock = new Mock<IClientRepository>();
            clientRepoMock.Setup(c => c.GetByIdAsync(client.Id)).ReturnsAsync(client);
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<ApplicationFormEntity>(It.IsAny<CreateApplicationFormDto>())).Returns(new ApplicationFormEntity());
            var logger = new NoopLogger<CreateApplicationFormHandler>();
            var handler = new CreateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);

            var dto = new CreateApplicationFormDto { ClientId = client.Id, ApplicationName = "NewApp" };
            var res = await handler.Handle(new CreateApplicationFormCommand(dto), CancellationToken.None);
            Assert.True(res.Success);
        }
        [Theory]
        [InlineData("","bad@x.com", true)]
        [InlineData(null,"bad@x.com", true)]
        [InlineData("Name","not-an-email", true)]
        [InlineData("Name","ok@ok.com", false)]
        [InlineData("","", false)]
        [InlineData(null,null,false)]
        [InlineData("N1","", true)]
        [InlineData("N2","also-not-email", true)]
        public async Task Create_Invalid_ValidationFails(string name, string email, bool accept)
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
            var r = await v.ValidateAsync(new CreateApplicationFormCommand(dto));
            Assert.False(r.IsValid);
        }
        [Theory]
        [InlineData("AppA1","a1@example.com", true)]
        [InlineData("AppA2","a2@example.com", true)]
        [InlineData("AppA3","a3@example.com", true)]
        [InlineData("AppA4","a4@example.com", true)]
        [InlineData("AppA5","a5@example.com", true)]
        [InlineData("AppA6","a6@example.com", true)]
        [InlineData("AppA7","a7@example.com", true)]
        [InlineData("AppA8","a8@example.com", true)]
        [InlineData("AppA9","a9@example.com", true)]
        [InlineData("AppA10","a10@example.com", true)]
        public async Task Create_Various_Valid_Succeeds(string name, string email, bool accept)
        {
            var repoMock = new Moq.Mock<IApplicationFormRepository>();
            var clientRepoMock = new Moq.Mock<IClientRepository>();
            var mapperMock = new Moq.Mock<AutoMapper.IMapper>();
            var logger = new NoopLogger<CreateApplicationFormHandler>();
            var client = new Client { Id = Guid.NewGuid(), Name = "C" };
            clientRepoMock.Setup(c => c.GetByIdAsync(client.Id)).ReturnsAsync(client);
            repoMock.Setup(r => r.GetByNameAndClientIdAsync(name, client.Id)).ReturnsAsync((ApplicationFormEntity)null);
            repoMock.Setup(r => r.AddAsync(It.IsAny<ApplicationFormEntity>()));
            repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            mapperMock.Setup(m => m.Map<ApplicationFormEntity>(It.IsAny<CreateApplicationFormDto>())).Returns(new ApplicationFormEntity());
            var handler = new CreateApplicationFormHandler(repoMock.Object, clientRepoMock.Object, mapperMock.Object, logger);
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
            var res = await handler.Handle(new CreateApplicationFormCommand(dto), CancellationToken.None);
            Assert.True(res.Success);
        }
    }
}
