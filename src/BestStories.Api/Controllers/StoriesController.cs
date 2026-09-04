using BestStories.Api.Extensions;
using BestStories.Service.Abstractions;
using BestStories.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace BestStories.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class StoriesController(IBestStoriesService bestStoriesService) : ControllerBase
{
    [HttpGet("best")]
    [ProducesResponseType<IReadOnlyList<BestStoryDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<BestStoryDto>>> GetBestStoriesAsync([FromQuery] int n = 10, CancellationToken cancellationToken = default)
    {
        var result = await bestStoriesService.GetBestStoriesAsync(n, cancellationToken);
        return result.ToActionResult();
    }
}
