using FluentValidation;
using IssueDesk.Application.Abstractions;
using IssueDesk.Application.Common.Mapping;
using IssueDesk.Domain.Entities;
using MediatR;

namespace IssueDesk.Application.Features.Tickets.Commands;

public sealed record AddCommentCommand(Guid TicketId, string Author, string Body) : IRequest<CommentDto>;

public sealed class AddCommentValidator : AbstractValidator<AddCommentCommand>
{
      public AddCommentValidator()
      {
            RuleFor(x => x.TicketId).NotEmpty();
            RuleFor(x => x.Author).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Body).NotEmpty().MinimumLength(3).MaximumLength(5000);
      }
}

public sealed class AddCommentHandler : IRequestHandler<AddCommentCommand, CommentDto>
{
      private readonly ITicketRepository _tickets;
      private readonly IUnitOfWork _uow;

      public AddCommentHandler(ITicketRepository tickets, IUnitOfWork uow)
          => (_tickets, _uow) = (tickets, uow);

      public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken ct)
      {
            // Ensure ticket exists (no tracking semantics needed here)
            var exists = await _tickets.GetByIdAsync(request.TicketId, ct)
                         ?? throw new KeyNotFoundException("Ticket not found.");

            var comment = new Comment
            {
                  Id = Guid.NewGuid(),
                  TicketId = request.TicketId,
                  Author = request.Author.Trim(),
                  Body = request.Body.Trim(),
                  CreatedAt = DateTime.UtcNow
            };

            await _tickets.AddCommentAsync(comment, ct);
            await _tickets.TouchAsync(request.TicketId, ct); // keep UpdatedAt accurate
            await _uow.SaveChangesAsync(ct);

            return comment.ToDto();
      }
}
