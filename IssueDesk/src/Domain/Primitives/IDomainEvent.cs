namespace IssueDesk.Domain.Primitives;

public interface IDomainEvent
{
      DateTime OccurredOnUtc { get; }
}
