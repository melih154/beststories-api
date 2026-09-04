namespace BestStories.Service.Models;

public sealed class HackerNewsItem
{
    public long Id { get; init; }

    public string? Title { get; init; }

    public string? Url { get; init; }

    public string? By { get; init; }

    public long Time { get; init; }

    public int Score { get; init; }

    public int Descendants { get; init; }

    public string? Type { get; init; }
}
