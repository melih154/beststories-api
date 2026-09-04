using BestStories.Service.Abstractions;
using BestStories.Service.Models;
using FluentValidation;

namespace BestStories.Service;

public sealed class BestStoriesService(
    IHackerNewsClient hackerNewsClient,
    IValidator<BestStoriesRequest> validator) : IBestStoriesService
{
    public async Task<ServiceResult<IReadOnlyList<BestStoryDto>>> GetBestStoriesAsync(
        int count,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(new BestStoriesRequest(count), cancellationToken);
        if (!validationResult.IsValid)
        {
            return ServiceResult<IReadOnlyList<BestStoryDto>>.Failure(validationResult);
        }

        var storyIds = await hackerNewsClient.GetBestStoryIdsAsync(cancellationToken);
        if (storyIds.Count == 0)
        {
            return ServiceResult<IReadOnlyList<BestStoryDto>>.Success([]);
        }

        var items = await GetItemsAsync(storyIds, cancellationToken);

        var stories = items
            .Where(item => item is not null)
            .Select(item => ToBestStory(item!))
            .Where(story => story is not null)
            .Select(story => story!)
            .OrderByDescending(story => story.Score)
            .Take(count)
            .ToArray();

        return ServiceResult<IReadOnlyList<BestStoryDto>>.Success(stories);
    }

    private async Task<IReadOnlyList<HackerNewsItem?>> GetItemsAsync(IReadOnlyList<long> storyIds, CancellationToken cancellationToken)
    {
        const int maxConcurrency = 20;
        using var semaphore = new SemaphoreSlim(maxConcurrency);

        var tasks = storyIds.Select(async storyId =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                return await hackerNewsClient.GetItemAsync(storyId, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        return await Task.WhenAll(tasks);
    }

    private static BestStoryDto? ToBestStory(HackerNewsItem item)
    {
        if (!string.Equals(item.Type, "story", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Url) || string.IsNullOrWhiteSpace(item.By))
        {
            return null;
        }

        return new BestStoryDto
        {
            Title = item.Title,
            Uri = item.Url,
            PostedBy = item.By,
            Time = DateTimeOffset.FromUnixTimeSeconds(item.Time),
            Score = item.Score,
            CommentCount = item.Descendants
        };
    }
}
