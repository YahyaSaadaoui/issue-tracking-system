using IssueDesk.Domain.Abstractions;
using IssueDesk.Domain.Enums;
using IssueDesk.Domain.Events;
using IssueDesk.Domain.Primitives;
using MediatR;

namespace IssueDesk.Domain.Entities;

public class Ticket : IHasDomainEvents
{
      // EF ctor
      public Ticket() { }

      // Properties (EF-friendly)
      public Guid Id { get; set; }
      public Guid ProjectId { get; set; }
      public Project? Project { get; set; }

      public string Title { get; set; } = string.Empty;
      public string? Description { get; set; }
      public TicketStatus Status { get; set; } = TicketStatus.New;
      public TicketPriority Priority { get; set; } = TicketPriority.Medium;
      public string? Assignee { get; set; }

      public DateTime CreatedAt { get; set; }
      public DateTime UpdatedAt { get; set; }

      public List<Comment> Comments { get; set; } = new();

      // Domain events
      private readonly List<INotification> _domainEvents = new();
      public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();
      public void ClearDomainEvents() => _domainEvents.Clear();
      private void Raise(INotification evt) => _domainEvents.Add(evt);

      // Factory
      public static Ticket Create(Guid projectId, string title, string? description, TicketPriority priority)
      {
            EnsureTitle(title);
            EnsureDescription(description);

            var now = DateTime.UtcNow;

            var t = new Ticket
            {
                  Id = Guid.NewGuid(),
                  ProjectId = projectId,
                  Title = title.Trim(),
                  Description = string.IsNullOrWhiteSpace(description) ? null : description!.Trim(),
                  Priority = priority,
                  Status = TicketStatus.New,
                  CreatedAt = now,
                  UpdatedAt = now
            };

            t.Raise(new TicketCreatedEvent(t.Id, t.ProjectId, t.Title));
            return t;
      }

      public void Assign(string assignee)
      {
            if (string.IsNullOrWhiteSpace(assignee))
                  throw new DomainException("Assignee must be provided.");

            var normalized = assignee.Trim();
            if (Assignee == normalized) return;

            Assignee = normalized;
            Touch();
            Raise(new TicketAssignedEvent(Id, normalized));
      }

      public void ChangeStatus(TicketStatus next)
      {
            if (next == Status)
                  throw new DomainException("Status is already set to the requested value.");

            var allowed = Status switch
            {
                  TicketStatus.New => TicketStatus.InProgress,
                  TicketStatus.InProgress => TicketStatus.Resolved,
                  TicketStatus.Resolved => TicketStatus.Closed,
                  TicketStatus.Closed => throw new DomainException("Closed tickets cannot change status."),
                  _ => throw new DomainException("Unknown status.")
            };

            if (next != allowed)
                  throw new DomainException($"Invalid transition: {Status} → {next}.");

            var prev = Status;
            Status = next;
            Touch();
            Raise(new TicketStatusChangedEvent(Id, prev, next));
      }

      public Comment AddComment(string author, string body)
      {
            if (string.IsNullOrWhiteSpace(author))
                  throw new DomainException("Author is required.");
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
            var t = (title ?? string.Empty).Trim();
            if (t.Length < 5 || t.Length > 120)
                  throw new DomainException("Title must be 5–120 characters.");
      }

      private static void EnsureDescription(string? description)
      {
            if (string.IsNullOrEmpty(description)) return;
            if (description.Length > 5000)
                  throw new DomainException("Description must be ≤ 5000 characters.");
      }

      private void Touch() => UpdatedAt = DateTime.UtcNow;
}
