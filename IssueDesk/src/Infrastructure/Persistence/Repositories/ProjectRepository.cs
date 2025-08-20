using IssueDesk.Application.Abstractions;
using IssueDesk.Domain.Entities;
using IssueDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IssueDesk.Infrastructure.Repositories;

internal sealed class ProjectRepository : IProjectRepository
{
      private readonly IssueDeskDbContext _db;
      public ProjectRepository(IssueDeskDbContext db) => _db = db;

      public async Task AddAsync(Project project, CancellationToken ct = default)
          => await _db.Projects.AddAsync(project, ct);

      public ValueTask<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
          => _db.Projects.FindAsync(new object?[] { id }, ct);

      public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken ct = default)
          => await _db.Projects.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct);
}
