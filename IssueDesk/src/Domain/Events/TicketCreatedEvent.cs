using MediatR;

namespace IssueDesk.Domain.Events;

public sealed record TicketCreatedEvent(Guid TicketId, Guid ProjectId, string Title) : INotification;
