using System.Text.Json.Serialization;
using IssueDesk.Application;
using IssueDesk.Application.Features.Projects.Commands;
using IssueDesk.Application.Features.Projects.Queries;
using IssueDesk.Application.Features.Tickets.Commands;
using IssueDesk.Application.Features.Tickets.Queries;
using IssueDesk.Domain.Enums;
using IssueDesk.Infrastructure;
using IssueDesk.Infrastructure.Persistence;
using IssueDesk.WebApi.Contracts;
using IssueDesk.WebApi.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();

// Use InMemory only for the special “Testing” environment (integration tests), otherwise full infra
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
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter())
);

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

// -------------------- API ROUTES --------------------
var api = app.MapGroup("/api").WithOpenApi();

// Projects
api.MapGet("/projects", async (IMediator mediator) =>
{
   var result = await mediator.Send(new GetProjectsQuery());
   return Results.Ok(result);
})
.WithName("GetProjects")
.WithTags("Projects");

api.MapPost("/projects", async (IMediator mediator, CreateProjectCommand cmd) =>
{
   var created = await mediator.Send(cmd);
   return Results.Created($"/api/projects/{created.Id}", created);
})
.WithName("CreateProject")
.WithTags("Projects");

// Tickets
api.MapPost("/tickets", async (IMediator mediator, CreateTicketCommand cmd) =>
{
   var created = await mediator.Send(cmd);
   return Results.Created($"/api/tickets/{created.Id}", created);
})
.WithName("CreateTicket")
.WithTags("Tickets");

api.MapGet("/tickets/{id:guid}", async (IMediator mediator, Guid id) =>
{
   var (ticket, comments) = await mediator.Send(new GetTicketByIdQuery(id));
   return Results.Ok(new { ticket, comments });
})
.WithName("GetTicketById")
.WithTags("Tickets");

api.MapGet("/projects/{projectId:guid}/tickets", async (
    IMediator mediator,
    Guid projectId,
    TicketStatus? status,
    TicketPriority? priority,
    string? assignee,
    string? search,
    int page = 1,
    int pageSize = 20) =>
{
   var result = await mediator.Send(
       new GetProjectTicketsQuery(projectId, status, priority, assignee, search, page, pageSize)
   );
   return Results.Ok(result);
})
.WithName("GetProjectTickets")
.WithTags("Tickets");

api.MapPost("/tickets/{id:guid}/assign", async (IMediator mediator, Guid id, AssignBody body) =>
{
   await mediator.Send(new AssignTicketCommand(id, body.Assignee));
   return Results.NoContent();
})
.WithName("AssignTicket")
.WithTags("Tickets");

api.MapPost("/tickets/{id:guid}/status", async (IMediator mediator, Guid id, ChangeStatusBody body) =>
{
   await mediator.Send(new ChangeTicketStatusCommand(id, body.NextStatus));
   return Results.NoContent();
})
.WithName("ChangeTicketStatus")
.WithTags("Tickets");

api.MapPost("/tickets/{id:guid}/comments", async (IMediator mediator, Guid id, AddCommentBody body) =>
{
   var created = await mediator.Send(new AddCommentCommand(id, body.Author, body.Body));
   return Results.Ok(created);
})
.WithName("AddComment")
.WithTags("Tickets");
// ------------------ END API ROUTES ------------------

// DB init
if (!isTesting)
{
   await app.MigrateAndSeedAsync();  // migrates + seeds relational DB
}
else
{
   using var scope = app.Services.CreateScope();
   var db = scope.ServiceProvider.GetRequiredService<IssueDeskDbContext>();
   await db.Database.EnsureCreatedAsync();
}

app.Run();

public partial class Program { }
