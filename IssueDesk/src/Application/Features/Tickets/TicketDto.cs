using IssueDesk.Domain.Enums;

namespace IssueDesk.Application.Features.Tickets;

public sealed record TicketDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    TicketStatus Status,
    TicketPriority Priority,
    string? Assignee,
    DateTime CreatedAt,
    DateTime UpdatedAt);
