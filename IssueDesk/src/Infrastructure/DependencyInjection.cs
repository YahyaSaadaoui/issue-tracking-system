using IssueDesk.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IssueDesk.Infrastructure;

public static class DependencyInjection
{
      public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
      {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=localhost,1433;Database=IssueDeskDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False;";

            services.AddDbContext<IssueDeskDbContext>(options =>
            {
                  options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
            });

            return services;
      }
}
