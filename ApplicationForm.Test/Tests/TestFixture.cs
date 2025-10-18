using Microsoft.Extensions.DependencyInjection;
using Application_Form.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application_Form.Application.Mappings;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Infrastructure.Repositories;
using Application_Form.Application.Services;
using AutoMapper;

namespace ApplicationForm.Test.Tests
{
    public class TestFixture
    {
        public IServiceProvider BuildServiceProvider(string dbName)
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(opts => opts.UseInMemoryDatabase(dbName));
            services.AddAutoMapper(typeof(ApplicationFormProfile).Assembly);
            services.AddScoped<IApplicationFormRepository, ApplicationFormRepository>();
            services.AddScoped<IApiCredentialService, ApiCredentialService>();
            return services.BuildServiceProvider();
        }
    }
}
