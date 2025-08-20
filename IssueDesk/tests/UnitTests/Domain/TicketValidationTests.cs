using FluentAssertions;
using IssueDesk.Domain.Entities;
using IssueDesk.Domain.Enums;
using IssueDesk.Domain.Primitives;
using Xunit;

namespace UnitTests.Domain;

public class TicketValidationTests
{
      [Fact]
      public void Title_Too_Short_Throws()
      {
            var act = () => Ticket.Create(Guid.NewGuid(), "1234", null, TicketPriority.Medium);
            act.Should().Throw<DomainException>();
      }

      [Fact]      
      public void Comment_Body_Too_Short_Throws()
      {
            var t = Ticket.Create(Guid.NewGuid(), "Valid title", null, TicketPriority.Low);
            var act = () => t.AddComment("me", "hi");
            act.Should().Throw<DomainException>();
      }
}
