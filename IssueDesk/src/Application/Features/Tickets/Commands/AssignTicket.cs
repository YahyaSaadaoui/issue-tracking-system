using FluentValidation;
using IssueDesk.Application.Abstractions;
using MediatR;

namespace IssueDesk.Application.Features.Tickets.Commands;

public sealed record AssignTicketCommand(Guid TicketId, string Assignee) : IRequest<Unit>;

public sealed class AssignTicketValidator : AbstractValidator<AssignTicketCommand>
{
      public AssignTicketValidator()
      {
            RuleFor(x => x.TicketId).NotEmpty();
            RuleFor(x => x.Assignee).NotEmpty().MinimumLength(1).MaximumLength(200);
      }
}

public sealed class AssignTicketHandler : IRequestHandler<AssignTicketCommand, Unit>
{
      private readonly ITicketRepository _tickets;
      private readonly IUnitOfWork _uow;

      public AssignTicketHandler(ITicketRepository tickets, IUnitOfWork uow)
          => (_tickets, _uow) = (tickets, uow);

      public async Task<Unit> Handle(AssignTicketCommand request, CancellationToken ct)
      {
            var ticket = await _tickets.GetByIdAsync(request.TicketId, ct)
                        ?? throw new KeyNotFoundException("Ticket not found.");

            ticket.Assign(request.Assignee);
            await _uow.SaveChangesAsync(ct);

            return Unit.Value;
      }
}
