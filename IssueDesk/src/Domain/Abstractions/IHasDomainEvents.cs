using MediatR;

namespace IssueDesk.Domain.Abstractions;

public interface IHasDomainEvents
{
      IReadOnlyCollection<INotification> DomainEvents { get; }
      void ClearDomainEvents();
}
