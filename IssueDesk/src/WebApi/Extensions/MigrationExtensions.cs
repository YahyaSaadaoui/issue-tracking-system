using IssueDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IssueDesk.WebApi.Extensions;

public static class MigrationExtensions
{
      public static async Task MigrateAndSeedAsync(this WebApplication app)
      {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IssueDeskDbContext>();

            await db.Database.MigrateAsync();

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
            }
      }
}
