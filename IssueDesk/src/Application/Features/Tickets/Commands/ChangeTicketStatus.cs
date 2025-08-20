using FluentValidation;
using IssueDesk.Application.Abstractions;
using IssueDesk.Domain.Enums;
using MediatR;

namespace IssueDesk.Application.Features.Tickets.Commands;

public sealed record ChangeTicketStatusCommand(Guid TicketId, TicketStatus NextStatus) : IRequest<Unit>;

public sealed class ChangeTicketStatusValidator : AbstractValidator<ChangeTicketStatusCommand>
{
      public ChangeTicketStatusValidator()
      {
            RuleFor(x => x.TicketId).NotEmpty();
            RuleFor(x => x.NextStatus).IsInEnum();
      }
}

public sealed class ChangeTicketStatusHandler : IRequestHandler<ChangeTicketStatusCommand, Unit>
{
      private readonly ITicketRepository _tickets;
      private readonly IUnitOfWork _uow;

      public ChangeTicketStatusHandler(ITicketRepository tickets, IUnitOfWork uow)
          => (_tickets, _uow) = (tickets, uow);

      public async Task<Unit> Handle(ChangeTicketStatusCommand request, CancellationToken ct)
      {
            var ticket = await _tickets.GetByIdAsync(request.TicketId, ct)
                        ?? throw new KeyNotFoundException("Ticket not found.");

            ticket.ChangeStatus(request.NextStatus);
            await _uow.SaveChangesAsync(ct);

            return Unit.Value;
      }
}
