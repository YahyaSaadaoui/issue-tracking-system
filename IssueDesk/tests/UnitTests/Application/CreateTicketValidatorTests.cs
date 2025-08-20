using FluentAssertions;
using IssueDesk.Application.Features.Tickets.Commands;
using IssueDesk.Domain.Enums;
using Xunit;

namespace UnitTests.Application;

public class CreateTicketValidatorTests
{
      [Fact]
      public void Invalid_Title_Should_Fail()
      {
            var v = new CreateTicketValidator();
            var result = v.Validate(new CreateTicketCommand(Guid.NewGuid(), "bad", null, TicketPriority.Low));
            result.IsValid.Should().BeFalse();
      }

      [Fact]
      public void Valid_Command_Should_Pass()
      {
            var v = new CreateTicketValidator();
            var result = v.Validate(new CreateTicketCommand(Guid.NewGuid(), "Valid title", "desc", TicketPriority.High));
            result.IsValid.Should().BeTrue();
      }
}
