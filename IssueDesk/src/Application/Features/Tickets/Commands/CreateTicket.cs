using FluentValidation;
using IssueDesk.Application.Abstractions;
using IssueDesk.Application.Common.Mapping;
using IssueDesk.Domain.Entities;
using IssueDesk.Domain.Enums;
using MediatR;

namespace IssueDesk.Application.Features.Tickets.Commands;

public sealed record CreateTicketCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    TicketPriority Priority) : IRequest<TicketDto>;

public sealed class CreateTicketValidator : AbstractValidator<CreateTicketCommand>
{
      public CreateTicketValidator()
      {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MinimumLength(5).MaximumLength(120);
            RuleFor(x => x.Description).MaximumLength(5000);
            RuleFor(x => x.Priority).IsInEnum();
      }
}

public sealed class CreateTicketHandler : IRequestHandler<CreateTicketCommand, TicketDto>
{
      private readonly ITicketRepository _tickets;
      private readonly IProjectRepository _projects;
      private readonly IUnitOfWork _uow;

      public CreateTicketHandler(ITicketRepository tickets, IProjectRepository projects, IUnitOfWork uow)
          => (_tickets, _projects, _uow) = (tickets, projects, uow);

      public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken ct)
      {
            var project = await _projects.GetByIdAsync(request.ProjectId, ct);
            if (project is null) throw new KeyNotFoundException("Project not found.");

            var ticket = Ticket.Create(request.ProjectId, request.Title, request.Description, request.Priority);
            await _tickets.AddAsync(ticket, ct);
            await _uow.SaveChangesAsync(ct);

            return ticket.ToDto();
      }
}
