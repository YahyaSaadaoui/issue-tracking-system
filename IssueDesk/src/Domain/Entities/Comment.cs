namespace IssueDesk.Domain.Entities;

public class Comment
{
      public Guid Id { get; set; }
      public Guid TicketId { get; set; }
      public Ticket? Ticket { get; set; }

      public string Author { get; set; } = string.Empty;
      public string Body { get; set; } = string.Empty;

      public DateTime CreatedAt { get; set; }
}
