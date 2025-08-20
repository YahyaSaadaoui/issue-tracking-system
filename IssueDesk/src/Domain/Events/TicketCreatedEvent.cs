using IssueDesk.Domain.Primitives;

namespace IssueDesk.Domain.Events;

public sealed record TicketCreatedEvent(Guid TicketId, Guid ProjectId) : DomainEvent;
