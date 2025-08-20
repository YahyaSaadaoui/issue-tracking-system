using IssueDesk.Application.Features.Projects;
using IssueDesk.Application.Features.Tickets;
using IssueDesk.Domain.Entities;

namespace IssueDesk.Application.Common.Mapping;

public static class MappingExtensions
{
      public static ProjectDto ToDto(this Project p) =>
          new(p.Id, p.Name, p.Key, p.CreatedAt);

      public static TicketDto ToDto(this Ticket t) =>
          new(t.Id, t.ProjectId, t.Title, t.Description, t.Status, t.Priority,
              t.Assignee, t.CreatedAt, t.UpdatedAt);

      public static CommentDto ToDto(this Comment c) =>
          new(c.Id, c.Author, c.Body, c.CreatedAt);
}
