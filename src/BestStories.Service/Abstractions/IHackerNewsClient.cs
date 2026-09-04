using BestStories.Service.Models;

namespace BestStories.Service.Abstractions;

public interface IHackerNewsClient
{
    Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken);

    Task<HackerNewsItem?> GetItemAsync(long id, CancellationToken cancellationToken);
}
