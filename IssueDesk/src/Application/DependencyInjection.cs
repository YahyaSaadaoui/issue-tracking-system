using FluentValidation;
using IssueDesk.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IssueDesk.Application;

public static class DependencyInjection
{
      public static IServiceCollection AddApplication(this IServiceCollection services)
      {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly));

            services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
      }
}
