using Application_Form.Application.Behaviors;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Application.Services;
using Application_Form.Domain.Entities;
using Application_Form.Infrastructure.Data;
using Application_Form.Infrastructure.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Filters;
using Application_Form.SwaggerDocs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register AutoMapper profiles from the application's assembly
builder.Services.AddAutoMapper(typeof(ApplicationForm).Assembly);

// Register FluentValidation validators from the application's assembly
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Register MediatR handlers from the application's assembly
builder.Services.AddMediatR(typeof(ApplicationForm).Assembly);

// Register validation behaviors for Result<T>
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationFormRepository, ApplicationFormRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IApiCredentialService, ApiCredentialService>();

// Disable the model-binding / ModelState validation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Configure Swagger and examples
builder.Services.AddSwaggerGen(c => c.ExampleFilters());
// Register example providers from this assembly
builder.Services.AddSwaggerExamplesFromAssemblyOf<OkRequestExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<BadRequestExample>();
// Include XML comments (generated via <GenerateDocumentationFile>true</GenerateDocumentationFile> in csproj)
var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
builder.Services.Configure<SwaggerGenOptions>(options =>
{
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!context.Clients.Any())
    {
        context.Clients.AddRange(
            new Client { Id = Guid.NewGuid(), Name = "Bank Misr" },
            new Client { Id = Guid.NewGuid(), Name = "QNB" },
            new Client { Id = Guid.NewGuid(), Name = "CIB" }
        );
        context.SaveChanges();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
