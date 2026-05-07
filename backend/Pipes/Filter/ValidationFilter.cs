using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace backend.Pipes.Filter;

/// <summary>
/// Action filter that runs FluentValidation validators for specified request types
/// before the controller action executes.
/// Apply to individual actions or whole controllers via
/// <c>[ValidationFilter(typeof(MyRequest))]</c>.
/// Returns HTTP 400 with a structured error body on validation failure:
/// <code>{ "message": "Validation failed", "errors": { "Field": ["message"] } }</code>
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidationFilterAttribute : ActionFilterAttribute
{
    private readonly Type[] _types;

    /// <summary>
    /// Initialises the filter with the request types whose validators should be run.
    /// </summary>
    /// <param name="types">One or more request DTO types registered with FluentValidation.</param>
    public ValidationFilterAttribute(params Type[] types)
    {
        _types = types;
    }

    /// <inheritdoc/>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var serviceProvider = context.HttpContext.RequestServices;

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;
            if (!_types.Contains(argument.GetType())) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (serviceProvider.GetService(validatorType) is not IValidator validator) continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = validator.Validate(validationContext);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                context.Result = new BadRequestObjectResult(new
                {
                    message = "Validation failed",
                    errors
                });

                return;
            }
        }
    }
}
