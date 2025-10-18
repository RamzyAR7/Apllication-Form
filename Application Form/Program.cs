using Application_Form.Domain.Entities;
using Application_Form.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentValidation;
using MediatR;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Infrastructure.Repositories;
using Application_Form.Application.Behaviors;
using Swashbuckle.AspNetCore.SwaggerGen;
using Application_Form.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register AutoMapper profiles from the application's assembly
builder.Services.AddAutoMapper(typeof(ApplicationForm).Assembly);

// Register FluentValidation validators from the application's assembly
builder.Services.AddValidatorsFromAssemblyContaining<ApplicationForm>();

// Register MediatR handlers from the application's assembly
builder.Services.AddMediatR(typeof(ApplicationForm).Assembly);

// Register validation behaviors for Result<T> and Result (non-generic) requests
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviorForResult<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviorForSimpleResult<,>));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationFormRepository, ApplicationFormRepository>();
builder.Services.AddScoped<IApiCredentialService, ApiCredentialService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
