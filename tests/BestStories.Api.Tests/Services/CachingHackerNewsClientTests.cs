using BestStories.Service;
using BestStories.Service.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace BestStories.Api.Tests.Services;

public sealed class CachingHackerNewsClientTests
{
    [Fact]
    public async Task GetBestStoryIdsAsync_UsesCache()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var innerClient = new Mock<HackerNewsClient>(new HttpClient());
        innerClient.Setup(x => x.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([1L, 2L]);

        var client = new CachingHackerNewsClient(cache, innerClient.Object);

        var first = await client.GetBestStoryIdsAsync(CancellationToken.None);
        var second = await client.GetBestStoryIdsAsync(CancellationToken.None);

        Assert.Equal(first, second);
        innerClient.Verify(x => x.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetItemAsync_UsesCachePerStoryId()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var innerClient = new Mock<HackerNewsClient>(new HttpClient());
        var item = new HackerNewsItem
        {
            Id = 42,
            Title = "title",
            Url = "https://example.com",
            By = "author",
            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Score = 10,
            Descendants = 5,
            Type = "story"
        };

        innerClient.Setup(x => x.GetItemAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var client = new CachingHackerNewsClient(cache, innerClient.Object);

        var first = await client.GetItemAsync(42, CancellationToken.None);
        var second = await client.GetItemAsync(42, CancellationToken.None);

        Assert.NotNull(first);
        Assert.Same(first, second);
        innerClient.Verify(x => x.GetItemAsync(42, It.IsAny<CancellationToken>()), Times.Once);
    }
}
