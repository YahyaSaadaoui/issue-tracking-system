namespace IssueDesk.Domain.Entities;

public class Project
{
      public Guid Id { get; set; }
      public string Name { get; set; } = string.Empty; // unique per account (enforced later)
      public string Key { get; set; } = string.Empty;  // e.g., "PAY"
      public DateTime CreatedAt { get; set; }

      public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
