using IssueDesk.Domain.Primitives;

namespace IssueDesk.Domain.Events;

public sealed record TicketAssignedEvent(Guid TicketId, string Assignee) : DomainEvent;
