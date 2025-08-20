using FluentAssertions;
using IssueDesk.Domain.Entities;
using IssueDesk.Domain.Enums;
using IssueDesk.Domain.Primitives;
using Xunit;

namespace UnitTests.Domain;

public class TicketStatusTransitionsTests
{
      [Fact]
      public void New_Can_Move_To_InProgress()
      {
            var t = Ticket.Create(Guid.NewGuid(), "Valid title", "desc", TicketPriority.Medium);
            t.ChangeStatus(TicketStatus.InProgress);
            t.Status.Should().Be(TicketStatus.InProgress);
      }

      [Fact]
      public void New_Cannot_Skip_To_Resolved()
      {
            var t = Ticket.Create(Guid.NewGuid(), "Valid title", null, TicketPriority.Low);
            var act = () => t.ChangeStatus(TicketStatus.Resolved);
            act.Should().Throw<DomainException>();
      }

      [Fact]
      public void InProgress_Can_Move_To_Resolved_Then_Closed()
      {
            var t = Ticket.Create(Guid.NewGuid(), "Valid title", null, TicketPriority.High);
            t.ChangeStatus(TicketStatus.InProgress);
            t.ChangeStatus(TicketStatus.Resolved);
            t.ChangeStatus(TicketStatus.Closed);
            t.Status.Should().Be(TicketStatus.Closed);
      }
}
