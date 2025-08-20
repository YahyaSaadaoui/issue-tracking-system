using IssueDesk.Domain.Enums;

namespace IssueDesk.WebApi.Contracts;

public sealed record AssignBody(string Assignee);
public sealed record ChangeStatusBody(TicketStatus NextStatus);
public sealed record AddCommentBody(string Author, string Body);
