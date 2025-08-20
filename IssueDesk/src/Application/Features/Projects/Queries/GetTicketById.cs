using IssueDesk.Application.Features.Tickets;
using IssueDesk.Application.Abstractions;
using IssueDesk.Application.Common.Mapping;
using MediatR;

namespace IssueDesk.Application.Features.Tickets.Queries;

public sealed record GetTicketByIdQuery(Guid Id) : IRequest<(TicketDto Ticket, IReadOnlyList<CommentDto> Comments)>;

public sealed class GetTicketByIdHandler
    : IRequestHandler<GetTicketByIdQuery, (TicketDto, IReadOnlyList<CommentDto>)>
{
      private readonly ITicketRepository _tickets;

      public GetTicketByIdHandler(ITicketRepository tickets) => _tickets = tickets;

      public async Task<(TicketDto, IReadOnlyList<CommentDto>)> Handle(GetTicketByIdQuery request, CancellationToken ct)
      {
            var ticket = await _tickets.GetByIdAsync(request.Id, ct)
                        ?? throw new KeyNotFoundException("Ticket not found.");

            var dto = ticket.ToDto();
            var comments = ticket.Comments.Select(c => c.ToDto()).OrderBy(c => c.CreatedAt).ToList();
            return (dto, comments);
      }
}
