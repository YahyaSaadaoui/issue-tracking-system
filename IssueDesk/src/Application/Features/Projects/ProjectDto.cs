namespace IssueDesk.Application.Features.Projects;

public sealed record ProjectDto(Guid Id, string Name, string Key, DateTime CreatedAt);
