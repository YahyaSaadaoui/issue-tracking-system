using IssueDesk.Application.Features.Projects;
using IssueDesk.Application.Abstractions;
using IssueDesk.Application.Common.Mapping;
using MediatR;

namespace IssueDesk.Application.Features.Projects.Queries;

public sealed record GetProjectsQuery() : IRequest<IReadOnlyList<ProjectDto>>;

public sealed class GetProjectsHandler : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectDto>>
{
      private readonly IProjectRepository _projects;

      public GetProjectsHandler(IProjectRepository projects) => _projects = projects;

      public async Task<IReadOnlyList<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken ct)
      {
            var all = await _projects.GetAllAsync(ct);
            return all.Select(p => p.ToDto()).OrderBy(p => p.Name).ToList();
      }
}
