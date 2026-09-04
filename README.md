# BestStories.Api

## Setup
- Install .NET 10 SDK
- Clone this repository

## Run locally
- `dotnet restore`
- `dotnet build BestStories.Api.slnx`
- `dotnet run --project src/BestStories.Api/BestStories.Api.csproj`

## Try the API
Open `src/BestStories.Api/BestStories.Api.http` in Visual Studio and run the request. Set `n` from 1 to 200.

## Test
- `dotnet test BestStories.Api.slnx`

## Assumptions
- The API uses Hacker News best stories IDs and then sorts fetched stories by score in descending order.
- Results with missing required story fields are excluded from the response.
- `n` must be between 1 and 200.

## Design decisions
- Controller-based ASP.NET Core API.
- In-memory caching for best story IDs and individual story items.
- Async calls with controlled concurrency to reduce pressure on Hacker News.

## Future improvements
- Add integration tests for endpoint behavior.
- Add cache settings from configuration.
- Add request metrics and simple rate limiting.
