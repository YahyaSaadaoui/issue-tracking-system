using FluentAssertions;
using IssueDesk.Application.Abstractions;
using IssueDesk.Application.Features.Tickets.Commands;
using IssueDesk.Domain.Entities;
using IssueDesk.Domain.Enums;
using Moq;
using Xunit;

namespace UnitTests.Application;

public class CreateTicketHandlerTests
{
      [Fact]
      public async Task Adds_Ticket_And_Returns_Dto()
      {
            var projectId = Guid.NewGuid();
            var proj = new Project { Id = projectId, Name = "P", Key = "P", CreatedAt = DateTime.UtcNow };

            var projects = new Mock<IProjectRepository>();
            projects.Setup(p => p.GetByIdAsync(projectId, It.IsAny<CancellationToken>())).ReturnsAsync(proj);

            Ticket? added = null;
            var tickets = new Mock<ITicketRepository>();
            tickets.Setup(t => t.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
                   .Callback<Ticket, CancellationToken>((t, _) => added = t)
                   .Returns(Task.CompletedTask);

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new CreateTicketHandler(tickets.Object, projects.Object, uow.Object);

            var dto = await handler.Handle(
                new CreateTicketCommand(projectId, "Valid title", "desc", TicketPriority.Medium),
                CancellationToken.None);

            added.Should().NotBeNull();
            dto.ProjectId.Should().Be(projectId);
            dto.Title.Should().Be("Valid title");
      }
}
