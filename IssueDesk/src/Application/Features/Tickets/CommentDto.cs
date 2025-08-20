namespace IssueDesk.Application.Features.Tickets;

public sealed record CommentDto(Guid Id, string Author, string Body, DateTime CreatedAt);
