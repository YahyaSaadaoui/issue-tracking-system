using IssueDesk.Domain.Enums;
using IssueDesk.Domain.Events;
using IssueDesk.Domain.Primitives;

namespace IssueDesk.Domain.Entities;

public class Ticket
{
      private readonly List<IDomainEvent> _domainEvents = new();
      public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

      public Guid Id { get; private set; }
      public Guid ProjectId { get; private set; }
      public Project? Project { get; private set; }

      public string Title { get; private set; } = string.Empty;
      public string? Description { get; private set; }

      public TicketStatus Status { get; private set; }
      public TicketPriority Priority { get; private set; }

      public string? Assignee { get; private set; }

      public DateTime CreatedAt { get; private set; }
      public DateTime UpdatedAt { get; private set; }

      public ICollection<Comment> Comments { get; private set; } = new List<Comment>();

      private Ticket() { } // EF

      public static Ticket Create(Guid projectId, string title, string? description, TicketPriority priority)
      {
            EnsureTitle(title);
            EnsureDescription(description);

            var now = DateTime.UtcNow;

            var ticket = new Ticket
            {
                  Id = Guid.NewGuid(),
                  ProjectId = projectId,
                  Title = title.Trim(),
                  Description = description?.Trim(),
                  Status = TicketStatus.New,
                  Priority = priority,
                  CreatedAt = now,
                  UpdatedAt = now
            };

            ticket.AddDomainEvent(new TicketCreatedEvent(ticket.Id, projectId));
            return ticket;
      }

      public void Assign(string assignee)
      {
            if (string.IsNullOrWhiteSpace(assignee))
                  throw new DomainException("Assignee is required.");

            var normalized = assignee.Trim();

            if (Assignee == normalized)
                  return; // no change → no event

            Assignee = normalized;
            Touch();
            AddDomainEvent(new TicketAssignedEvent(Id, normalized));
      }

      public void ChangeStatus(TicketStatus next)
      {
            if (!IsValidTransition(Status, next))
                  throw new DomainException($"Invalid status transition: {Status} → {next}.");

            var old = Status;
            Status = next;
            Touch();
            AddDomainEvent(new TicketStatusChangedEvent(Id, old, next));
      }

      public Comment AddComment(string author, string body)
      {
            if (string.IsNullOrWhiteSpace(author))
                  throw new DomainException("Comment author is required.");

            if (string.IsNullOrWhiteSpace(body) || body.Trim().Length < 3)
                  throw new DomainException("Comment body must be at least 3 characters.");

            var c = new Comment
            {
                  Id = Guid.NewGuid(),
                  TicketId = Id,
                  Author = author.Trim(),
                  Body = body.Trim(),
                  CreatedAt = DateTime.UtcNow
            };

            Comments.Add(c);
            Touch();
            return c;
      }

      private static void EnsureTitle(string title)
      {
            if (string.IsNullOrWhiteSpace(title))
                  throw new DomainException("Title is required.");

            var len = title.Trim().Length;
            if (len < 5 || len > 120)
                  throw new DomainException("Title must be between 5 and 120 characters.");
      }

      private static void EnsureDescription(string? description)
      {
            if (description is null) return;
            if (description.Length > 5000)
                  throw new DomainException("Description must be 0–5000 characters.");
      }

      private static bool IsValidTransition(TicketStatus current, TicketStatus next) =>
          (current, next) switch
          {
                (TicketStatus.New, TicketStatus.InProgress) => true,
                (TicketStatus.InProgress, TicketStatus.Resolved) => true,
                (TicketStatus.Resolved, TicketStatus.Closed) => true,
                _ => current == next // allow no-op
          };

      private void Touch() => UpdatedAt = DateTime.UtcNow;

      private void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);
}
