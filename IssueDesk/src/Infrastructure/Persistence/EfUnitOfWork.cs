using IssueDesk.Application.Abstractions;

namespace IssueDesk.Infrastructure.Persistence;

internal sealed class EfUnitOfWork : IUnitOfWork
{
      private readonly IssueDeskDbContext _db;
      public EfUnitOfWork(IssueDeskDbContext db) => _db = db;

      public Task<int> SaveChangesAsync(CancellationToken ct = default)
          => _db.SaveChangesAsync(ct);
}
