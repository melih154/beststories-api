using System.Net.Http.Json;
using BestStories.Service.Abstractions;
using BestStories.Service.Models;

namespace BestStories.Service;

public class HackerNewsClient(HttpClient httpClient) : IHackerNewsClient
{
    public virtual async Task<IReadOnlyList<long>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        var storyIds = await httpClient.GetFromJsonAsync<long[]>("v0/beststories.json", cancellationToken);
        return storyIds ?? [];
    }

    public virtual Task<HackerNewsItem?> GetItemAsync(long id, CancellationToken cancellationToken)
    {
        return httpClient.GetFromJsonAsync<HackerNewsItem>($"v0/item/{id}.json", cancellationToken);
    }
}
