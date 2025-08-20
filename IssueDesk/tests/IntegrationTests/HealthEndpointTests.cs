using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
      private readonly WebApplicationFactory<Program> _factory;

      public HealthEndpointTests(WebApplicationFactory<Program> factory)
      {
            _factory = factory.WithWebHostBuilder(b => b.UseEnvironment("Testing"));
      }

      [Fact]
      public async Task Health_Returns_200()
      {
            var client = _factory.CreateClient();
            var res = await client.GetAsync("/health");
            res.StatusCode.Should().Be(HttpStatusCode.OK);
      }
}
