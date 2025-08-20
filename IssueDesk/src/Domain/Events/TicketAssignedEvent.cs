using MediatR;

namespace IssueDesk.Domain.Events;

public sealed record TicketAssignedEvent(Guid TicketId, string Assignee) : INotification;
