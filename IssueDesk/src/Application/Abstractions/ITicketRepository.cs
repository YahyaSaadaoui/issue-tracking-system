using IssueDesk.Domain.Entities;
using IssueDesk.Domain.Enums;

namespace IssueDesk.Application.Abstractions;

public sealed record TicketFilter(
    TicketStatus? Status,
    TicketPriority? Priority,
    string? Assignee,
    string? Search,
    int Page,
    int PageSize);

public interface ITicketRepository
{
      Task AddAsync(Ticket ticket, CancellationToken ct = default);
      ValueTask<Ticket?> GetByIdAsync(Guid id, CancellationToken ct = default);

      Task<(IReadOnlyList<Ticket> Items, int Total)> GetByProjectAsync(
          Guid projectId, TicketFilter filter, CancellationToken ct = default);
}
