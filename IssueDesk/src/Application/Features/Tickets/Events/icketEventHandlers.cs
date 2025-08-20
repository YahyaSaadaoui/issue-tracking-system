using IssueDesk.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IssueDesk.Application.Features.Tickets.Events;

public sealed class TicketCreatedHandler(ILogger<TicketCreatedHandler> logger) : INotificationHandler<TicketCreatedEvent>
{
      public Task Handle(TicketCreatedEvent notification, CancellationToken cancellationToken)
      {
            logger.LogInformation("Ticket created: {Id} in Project {ProjectId} - {Title}", notification.TicketId, notification.ProjectId, notification.Title);
            return Task.CompletedTask;
      }
}

public sealed class TicketAssignedHandler(ILogger<TicketAssignedHandler> logger) : INotificationHandler<TicketAssignedEvent>
{
      public Task Handle(TicketAssignedEvent notification, CancellationToken cancellationToken)
      {
            logger.LogInformation("Ticket {Id} assigned to {Assignee}", notification.TicketId, notification.Assignee);
            return Task.CompletedTask;
      }
}

public sealed class TicketStatusChangedHandler(ILogger<TicketStatusChangedHandler> logger) : INotificationHandler<TicketStatusChangedEvent>
{
      public Task Handle(TicketStatusChangedEvent notification, CancellationToken cancellationToken)
      {
            logger.LogInformation("Ticket {Id} status changed: {Old} -> {New}", notification.TicketId, notification.OldStatus, notification.NewStatus);
            return Task.CompletedTask;
      }
}
