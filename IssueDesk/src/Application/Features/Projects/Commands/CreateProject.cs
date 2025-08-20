using FluentValidation;
using IssueDesk.Application.Abstractions;
using IssueDesk.Application.Features.Projects;
using IssueDesk.Domain.Entities;
using MediatR;

namespace IssueDesk.Application.Features.Projects.Commands;

public sealed record CreateProjectCommand(string Name, string Key) : IRequest<ProjectDto>;

public sealed class CreateProjectValidator : AbstractValidator<CreateProjectCommand>
{
      public CreateProjectValidator()
      {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Key).NotEmpty().MaximumLength(10);
      }
}

public sealed class CreateProjectHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
      private readonly IProjectRepository _projects;
      private readonly IUnitOfWork _uow;

      public CreateProjectHandler(IProjectRepository projects, IUnitOfWork uow)
          => (_projects, _uow) = (projects, uow);

      public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken ct)
      {
            var p = new Project
            {
                  Id = Guid.NewGuid(),
                  Name = request.Name.Trim(),
                  Key = request.Key.Trim().ToUpperInvariant(),
                  CreatedAt = DateTime.UtcNow
            };

            await _projects.AddAsync(p, ct);
            await _uow.SaveChangesAsync(ct);

            return new ProjectDto(p.Id, p.Name, p.Key, p.CreatedAt);
      }
}
