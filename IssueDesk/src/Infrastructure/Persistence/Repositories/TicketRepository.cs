using IssueDesk.Application.Abstractions;
using IssueDesk.Domain.Entities;
using IssueDesk.Domain.Enums;
using IssueDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IssueDesk.Infrastructure.Repositories;

internal sealed class TicketRepository : ITicketRepository
{
    private readonly IssueDeskDbContext _db;
    public TicketRepository(IssueDeskDbContext db) => _db = db;

    public async Task AddAsync(Ticket ticket, CancellationToken ct = default)
        => await _db.Tickets.AddAsync(ticket, ct);

    public async ValueTask<Ticket?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Tickets
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<(IReadOnlyList<Ticket> Items, int Total)> GetByProjectAsync(
        Guid projectId, TicketFilter filter, CancellationToken ct = default)

    {
        var q = _db.Tickets.AsNoTracking().Where(t => t.ProjectId == projectId);

        if (filter.Status.HasValue) q = q.Where(t => t.Status == filter.Status);
        if (filter.Priority.HasValue) q = q.Where(t => t.Priority == filter.Priority);
        if (!string.IsNullOrWhiteSpace(filter.Assignee))
        {
            var a = filter.Assignee.Trim();
            q = q.Where(t => t.Assignee != null && t.Assignee.Contains(a));
        }
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.Trim();
            q = q.Where(t => t.Title.Contains(s) || (t.Description != null && t.Description.Contains(s)));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(t => t.UpdatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }
    public async Task AddCommentAsync(Comment comment, CancellationToken ct = default)
      => await _db.Comments.AddAsync(comment, ct);

    public async Task TouchAsync(Guid ticketId, CancellationToken ct = default)
    {
        await _db.Tickets
            .Where(t => t.Id == ticketId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(t => t.UpdatedAt, _ => DateTime.UtcNow), ct);
    }
}
