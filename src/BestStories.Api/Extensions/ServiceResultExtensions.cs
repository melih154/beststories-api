using BestStories.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace BestStories.Api.Extensions;

public static class ServiceResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this ServiceResult<T> result)
    {
        if (result.ValidationResult.IsValid)
        {
            return new OkObjectResult(result.Value);
        }

        var errors = result.ValidationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        return new BadRequestObjectResult(new ValidationProblemDetails(errors));
    }
}
