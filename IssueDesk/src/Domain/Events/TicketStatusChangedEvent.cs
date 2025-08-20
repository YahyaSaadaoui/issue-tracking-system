using IssueDesk.Domain.Enums;
using MediatR;

namespace IssueDesk.Domain.Events;

public sealed record TicketStatusChangedEvent(Guid TicketId, TicketStatus OldStatus, TicketStatus NewStatus) : INotification;
