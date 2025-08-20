using System.Reflection;
using FluentValidation;
using IssueDesk.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IssueDesk.Application;

public static class DependencyInjection
{
      public static IServiceCollection AddApplication(this IServiceCollection services)
      {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
      }
}
