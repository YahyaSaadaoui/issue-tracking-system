using FluentValidation;
using IssueDesk.Domain.Primitives;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IssueDesk.WebApi.Extensions;

public static class ExceptionHandlingExtensions
{
      public static void AddProblemDetailsServices(this IServiceCollection services) =>
          services.AddProblemDetails();

      public static void UseProblemDetailsExceptionHandler(this IApplicationBuilder app)
      {
            app.UseExceptionHandler(errorApp =>
            {
                  errorApp.Run(async context =>
              {
                      var feature = context.Features.Get<IExceptionHandlerFeature>();
                      var ex = feature?.Error;

                      (int status, string title, IDictionary<string, object?>? extensions) = ex switch
                      {
                            ValidationException ve => (StatusCodes.Status400BadRequest, "Validation failed",
                            new Dictionary<string, object?> { ["errors"] = ve.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) }),

                            DomainException de => (StatusCodes.Status400BadRequest, "Domain rule violated",
                            new Dictionary<string, object?> { ["detail"] = de.Message }),

                            KeyNotFoundException knf => (StatusCodes.Status404NotFound, "Resource not found", null),

                            _ => (StatusCodes.Status500InternalServerError, "Unexpected error", null)
                      };

                      var problem = new ProblemDetails
                      {
                            Title = title,
                            Status = status,
                            Type = $"https://httpstatuses.com/{status}",
                            Detail = ex?.Message
                      };

                      if (extensions is not null)
                            foreach (var kv in extensions) problem.Extensions[kv.Key] = kv.Value;

                      context.Response.ContentType = "application/problem+json";
                      context.Response.StatusCode = status;
                      await context.Response.WriteAsJsonAsync(problem);
                });
            });
      }
}
