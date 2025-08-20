using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IssueDesk.Infrastructure.Persistence;

public class IssueDeskDbContextFactory : IDesignTimeDbContextFactory<IssueDeskDbContext>
{
    public IssueDeskDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("DefaultConnection")
            ?? "Server=localhost,1433;Database=IssueDeskDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False;";

        var builder = new DbContextOptionsBuilder<IssueDeskDbContext>();
        builder.UseSqlServer(cs);
        return new IssueDeskDbContext(builder.Options);
    }
}
