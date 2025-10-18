using Xunit;
using Application_Form.Application.Mappings;
using AutoMapper;
using Application_Form.Application.DTOs;
using Application_Form.Domain.Entities;
using Application_Form.Domain.Constant;

namespace ApplicationForm.Test.Tests
{
    public class MappingTests
    {
        private IMapper CreateMapper()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile(new ApplicationFormProfile()));
            return cfg.CreateMapper();
        }

        [Fact]
        public void UpdateMapping_Ignores_ClientId()
        {
            var mapper = CreateMapper();
            var entity = new Application_Form.Domain.Entities.ApplicationForm { ClientId = System.Guid.NewGuid(), ApplicationName = "Before" };
            var dto = new UpdateApplicationFormDto { ApplicationName = "After", ApplicationDescription = "D", EmailAddress = "a@b.com", OrganizationName = "Org", ApplicationType = "X", Environment = ApiEnvironment.Sandbox.ToString() };
            mapper.Map(dto, entity);
            Assert.Equal("After", entity.ApplicationName);
            Assert.NotEqual(System.Guid.Empty, entity.ClientId);
        }
    }
}
