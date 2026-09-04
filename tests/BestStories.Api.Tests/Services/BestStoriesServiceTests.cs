using AutoFixture;
using AutoFixture.AutoMoq;
using BestStories.Service;
using BestStories.Service.Abstractions;
using BestStories.Service.Models;
using BestStories.Service.Validation;
using Moq;

namespace BestStories.Api.Tests.Services;

public sealed class BestStoriesServiceTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    [Fact]
    public async Task GetBestStoriesAsync_ReturnsTopNStoriesSortedByScore()
    {
        var client = new Mock<IHackerNewsClient>();
        client.Setup(x => x.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([1, 2, 3]);

        client.Setup(x => x.GetItemAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(CreateStory(1, 10));
        client.Setup(x => x.GetItemAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(CreateStory(2, 30));
        client.Setup(x => x.GetItemAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(CreateStory(3, 20));

        var service = CreateService(client.Object);

        var result = await service.GetBestStoriesAsync(2, CancellationToken.None);

        Assert.True(result.ValidationResult.IsValid);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal(30, result.Value[0].Score);
        Assert.Equal(20, result.Value[1].Score);
    }

    [Fact]
    public async Task GetBestStoriesAsync_FiltersInvalidItems()
    {
        var client = new Mock<IHackerNewsClient>();
        client.Setup(x => x.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([1, 2, 3, 4]);

        client.Setup(x => x.GetItemAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(CreateStory(1, 50));
        client.Setup(x => x.GetItemAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync((HackerNewsItem?)null);
        client.Setup(x => x.GetItemAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(CreateStory(3, 100, type: "job"));
        client.Setup(x => x.GetItemAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(CreateStory(4, 40, url: ""));

        var service = CreateService(client.Object);

        var result = await service.GetBestStoriesAsync(10, CancellationToken.None);

        Assert.True(result.ValidationResult.IsValid);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
        Assert.Equal(1, result.Value[0].CommentCount);
    }

    [Fact]
    public async Task GetBestStoriesAsync_ReturnsEmptyWhenNoStoryIds()
    {
        var client = new Mock<IHackerNewsClient>();
        client.Setup(x => x.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var service = CreateService(client.Object);

        var result = await service.GetBestStoriesAsync(5, CancellationToken.None);

        Assert.True(result.ValidationResult.IsValid);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
        client.Verify(x => x.GetItemAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public async Task GetBestStoriesAsync_ReturnsValidationFailureForInvalidCount(int count)
    {
        var client = new Mock<IHackerNewsClient>();
        var service = CreateService(client.Object);

        var result = await service.GetBestStoriesAsync(count, CancellationToken.None);

        Assert.False(result.ValidationResult.IsValid);
        Assert.Null(result.Value);
        var error = Assert.Single(result.ValidationResult.Errors);
        Assert.Equal("n", error.PropertyName);
        Assert.Equal("n must be between 1 and 200.", error.ErrorMessage);
        client.Verify(x => x.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static BestStoriesService CreateService(IHackerNewsClient client) =>
        new(client, new BestStoriesRequestValidator());

    private HackerNewsItem CreateStory(long id, int score, string type = "story", string? url = null)
    {
        var title = _fixture.Create<string>();
        var postedBy = _fixture.Create<string>();
        var storyUrl = url ?? $"https://example.com/{id}";

        return new HackerNewsItem
        {
            Id = id,
            Title = title,
            Url = storyUrl,
            By = postedBy,
            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Score = score,
            Descendants = 1,
            Type = type
        };
    }
}
