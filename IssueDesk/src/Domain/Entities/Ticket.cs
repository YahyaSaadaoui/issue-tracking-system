using IssueDesk.Domain.Enums;

namespace IssueDesk.Domain.Entities;

public class Ticket
{
      public Guid Id { get; set; }
      public Guid ProjectId { get; set; }
      public Project? Project { get; set; }

      public string Title { get; set; } = string.Empty;
      public string? Description { get; set; }

      public TicketStatus Status { get; set; }
      public TicketPriority Priority { get; set; }

      // username/email; keep simple for MVP
      public string? Assignee { get; set; }

      public DateTime CreatedAt { get; set; }
      public DateTime UpdatedAt { get; set; }

      public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
