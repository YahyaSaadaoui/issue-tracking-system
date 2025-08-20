using IssueDesk.Domain.Entities;

namespace IssueDesk.Application.Abstractions;

public interface IProjectRepository
{
      Task AddAsync(Project project, CancellationToken ct = default);
      ValueTask<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
      Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken ct = default);
}
