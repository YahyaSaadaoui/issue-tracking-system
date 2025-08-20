using IssueDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IssueDesk.WebApi.Extensions;

public static class MigrationExtensions
{
      public static async Task MigrateAndSeedAsync(this WebApplication app)
      {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IssueDeskDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbMigrations");

            const int maxAttempts = 10;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                  try
                  {
                        await db.Database.MigrateAsync();
                        break;
                  }
                  catch (Exception ex)
                  {
                        if (attempt == maxAttempts)
                        {
                              logger.LogError(ex, "Failed to apply migrations after {Attempts} attempts.", maxAttempts);
                              throw;
                        }

                        logger.LogWarning(ex, "DB not ready yet (attempt {Attempt}/{Max}). Retrying in 2s...", attempt, maxAttempts);
                        await Task.Delay(TimeSpan.FromSeconds(2));
                  }
            }

            // Seed one project if none exists
            if (!await db.Projects.AnyAsync())
            {
                  db.Projects.Add(new IssueDesk.Domain.Entities.Project
                  {
                        Id = Guid.NewGuid(),
                        Name = "Payments",
                        Key = "PAY",
                        CreatedAt = DateTime.UtcNow
                  });
                  await db.SaveChangesAsync();
                  logger.LogInformation("Seeded default project 'Payments (PAY)'.");
            }
      }
}
