using IssueDesk.Domain.Enums;
using IssueDesk.Domain.Primitives;

namespace IssueDesk.Domain.Events;

public sealed record TicketStatusChangedEvent(Guid TicketId, TicketStatus OldStatus, TicketStatus NewStatus) : DomainEvent;
