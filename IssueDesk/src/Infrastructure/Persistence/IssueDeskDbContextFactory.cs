using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IssueDesk.Infrastructure.Persistence;

public sealed class IssueDeskDbContextFactory : IDesignTimeDbContextFactory<IssueDeskDbContext>
{
      public IssueDeskDbContext CreateDbContext(string[] args)
      {
            // When running: CWD is usually .../IssueDesk/src/Infrastructure
            var infraDir = Directory.GetCurrentDirectory();
            var repoRoot = Path.GetFullPath(Path.Combine(infraDir, "../../")); // -> .../IssueDesk
            var webApiDir = Path.Combine(repoRoot, "src", "WebApi");

            var config = new ConfigurationBuilder()
                .SetBasePath(webApiDir)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection")
                ?? "Server=localhost,1433;Database=IssueDeskDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False;";

            var options = new DbContextOptionsBuilder<IssueDeskDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new IssueDeskDbContext(options);
      }
}
