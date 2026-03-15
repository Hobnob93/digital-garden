using DigitalGarden.Data;
using DigitalGarden.Extensions;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Models.Options;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DigitalGarden.Services.Implementations;

public class LifeDataProvider : ILifeDataProvider
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly LastFmOptions _lastFmOptions;

    public LifeDataProvider(ApplicationDbContext dbContext, IHttpClientFactory httpClientFactory, IOptions<LastFmOptions> lastFmOptions)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _lastFmOptions = lastFmOptions.Value;
    }

    public async Task<FamousQuote> GetQuoteOfTheDayAsync()
    {
        var allQuotes = await _dbContext.FamousQuotes.ToListAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;

        var random = new Random(today);
        var randomIndex = random.Next(allQuotes.Count);

        return allQuotes[randomIndex].ToDomain();
    }

    public async Task<RecentLifeLog[]> GetRecentLifeLogsAsync()
    {
        var allRecentLifeLogs = await _dbContext.RecentLifeLogItems.ToListAsync();

        return allRecentLifeLogs
            .Select(l => l.ToDomain())
            .ToArray();
    }

    public async Task<LastFmTopArtistsResponse> GetLastFmTopArtists()
    {
        var client = _httpClientFactory.CreateClient(ApiConstants.LastFmClientName);

        var endpoint = string.Format(_lastFmOptions.TopArtistsEndpoint, _lastFmOptions.UserId, _lastFmOptions.ApiKey);
        var result = await client.GetAsync(endpoint);
        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<LastFmTopArtistsResponse>()
            ?? throw new InvalidDataException($"GET result for endpoint '{endpoint}' could not be parsed!");
    }

    public async Task<LastFmTopTracksResponse> GetLastFmTopTracks()
    {
        var client = _httpClientFactory.CreateClient(ApiConstants.LastFmClientName);

        var endpoint = string.Format(_lastFmOptions.TopTracksEndpoint, _lastFmOptions.UserId, _lastFmOptions.ApiKey);
        var result = await client.GetAsync(endpoint);
        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<LastFmTopTracksResponse>()
            ?? throw new InvalidDataException($"GET result for endpoint '{endpoint}' could not be parsed!");
    }
}
