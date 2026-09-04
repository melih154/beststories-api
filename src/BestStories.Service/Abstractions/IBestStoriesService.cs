using BestStories.Service.Models;

namespace BestStories.Service.Abstractions;

public interface IBestStoriesService
{
    Task<ServiceResult<IReadOnlyList<BestStoryDto>>> GetBestStoriesAsync(int count, CancellationToken cancellationToken);
}
