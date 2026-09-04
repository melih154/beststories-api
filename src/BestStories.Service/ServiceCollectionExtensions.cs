using BestStories.Service.Abstractions;
using BestStories.Service.Models;
using BestStories.Service.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace BestStories.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBestStoriesServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHttpClient<HackerNewsClient>(client =>
        {
            client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/");
        })
        .AddResilienceHandler("retry", pipeline =>
        {
            pipeline.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(200),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true
            });
        });
        services.AddScoped<IHackerNewsClient, CachingHackerNewsClient>();
        services.AddScoped<IValidator<BestStoriesRequest>, BestStoriesRequestValidator>();
        services.AddScoped<IBestStoriesService, BestStoriesService>();

        return services;
    }
}
