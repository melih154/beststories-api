using BestStories.Api.Extensions;
using BestStories.Service.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BestStories.Api.Tests.Extensions;

public sealed class ServiceResultExtensionsTests
{
    [Fact]
    public void ToActionResult_ReturnsOkWithValueForValidResult()
    {
        IReadOnlyList<BestStoryDto> stories = [];
        var serviceResult = ServiceResult<IReadOnlyList<BestStoryDto>>.Success(stories);

        var actionResult = serviceResult.ToActionResult();

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.Same(stories, okResult.Value);
    }

    [Fact]
    public void ToActionResult_ReturnsValidationProblemForInvalidResult()
    {
        var validationResult = new ValidationResult([
            new ValidationFailure("n", "n must be between 1 and 200.")
        ]);
        var serviceResult = ServiceResult<IReadOnlyList<BestStoryDto>>.Failure(validationResult);

        var actionResult = serviceResult.ToActionResult();

        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        var problem = Assert.IsType<ValidationProblemDetails>(badRequest.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.Equal(["n must be between 1 and 200."], problem.Errors["n"]);
    }
}
