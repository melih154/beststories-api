using BestStories.Service.Abstractions;
using BestStories.Service.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BestStories.Service;

public sealed class CachingHackerNewsClient(IMemoryCache cache, HackerNewsClient innerClient) : IHackerNewsClient
{
    private static readonly TimeSpan BestStoryIdsCacheDuration = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan StoryItemCacheDuration = TimeSpan.FromMinutes(5);

    public async Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        var storyIds = await cache.GetOrCreateAsync(
            "hackernews:beststoryids",
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = BestStoryIdsCacheDuration;
                return await innerClient.GetBestStoryIdsAsync(cancellationToken);
            });

        return storyIds ?? [];
    }

    public Task<HackerNewsItem?> GetItemAsync(long id, CancellationToken cancellationToken)
    {
        return cache.GetOrCreateAsync(
            $"hackernews:item:{id}",
            async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = StoryItemCacheDuration;
                return await innerClient.GetItemAsync(id, cancellationToken);
            });
    }
}
