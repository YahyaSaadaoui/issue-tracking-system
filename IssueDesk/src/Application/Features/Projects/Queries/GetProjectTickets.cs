using IssueDesk.Application.Abstractions;
using IssueDesk.Application.Common;
using IssueDesk.Application.Common.Mapping;
using IssueDesk.Application.Features.Tickets;
using IssueDesk.Domain.Enums;
using MediatR;

namespace IssueDesk.Application.Features.Tickets.Queries;

public sealed record GetProjectTicketsQuery(
    Guid ProjectId,
    TicketStatus? Status,
    TicketPriority? Priority,
    string? Assignee,
    string? Search,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<TicketDto>>;

public sealed class GetProjectTicketsHandler : IRequestHandler<GetProjectTicketsQuery, PagedResult<TicketDto>>
{
      private readonly ITicketRepository _tickets;

      public GetProjectTicketsHandler(ITicketRepository tickets) => _tickets = tickets;

      public async Task<PagedResult<TicketDto>> Handle(GetProjectTicketsQuery request, CancellationToken ct)
      {
            var filter = new TicketFilter(request.Status, request.Priority, request.Assignee, request.Search,
                                          Math.Max(1, request.Page), Math.Clamp(request.PageSize, 1, 200));

            var (items, total) = await _tickets.GetByProjectAsync(request.ProjectId, filter, ct);
            var dtos = items.Select(t => t.ToDto()).ToList();

            return new PagedResult<TicketDto>
            {
                  Items = dtos,
                  Total = total,
                  Page = filter.Page,
                  PageSize = filter.PageSize
            };
      }
}
