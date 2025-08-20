using System.Text.Json.Serialization;
using IssueDesk.Application;
using IssueDesk.Application.Features.Projects;
using IssueDesk.Application.Features.Projects.Commands;
using IssueDesk.Application.Features.Projects.Queries;
using IssueDesk.Application.Features.Tickets;
using IssueDesk.Application.Features.Tickets.Commands;
using IssueDesk.Application.Features.Tickets.Queries;
using IssueDesk.Domain.Enums;
using IssueDesk.Infrastructure;
using IssueDesk.WebApi.Extensions;
using MediatR;
using IssueDesk.WebApi.Contracts;
using Serilog;
using IssueDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;                   


var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, cfg) => { cfg.ReadFrom.Configuration(ctx.Configuration); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();

// 👇 Only use SQL Server in non-testing; InMemory in tests
var isTesting = builder.Environment.IsEnvironment("Testing");
if (isTesting)
{
builder.Services.AddDbContext<IssueDeskDbContext>(opt => opt.UseInMemoryDatabase("itest-db"));
}
else
{
builder.Services.AddInfrastructure(builder.Configuration);
}

builder.Services.AddProblemDetailsServices();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseProblemDetailsExceptionHandler();

if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

// Health
app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
   .WithName("Health")
   .WithTags("Health")
   .WithOpenApi();

// API routes … (unchanged)

// Apply migrations / seed (skip relational migration in tests)
if (!isTesting)
{
await app.MigrateAndSeedAsync();
}
else
{
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<IssueDeskDbContext>();
await db.Database.EnsureCreatedAsync();
}

app.Run();

public partial class Program { }