using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;

namespace DigitalGarden.Client.Services.Implementations;

public class LifeDataClient : BaseClient, ILifeDataProvider
{
    public LifeDataClient(IHttpClientFactory httpFactory)
        : base(httpFactory)
    {
    }

    public async Task<FamousQuote> GetQuoteOfTheDayAsync()
    {
        var apiResult = await Get<FamousQuote>("Life/QuoteOfTheDay");
        return apiResult.AsExpectedType;
    }

    public async Task<RecentLifeLog[]> GetRecentLifeLogsAsync()
    {
        var apiResult = await Get<RecentLifeLog[]>("Life/RecentLifeLogs");
        return apiResult.AsExpectedType;
    }

    public async Task<LastFmTopArtistsResponse> GetLastFmTopArtistsAsync()
    {
        var apiResult = await Get<LastFmTopArtistsResponse>("Life/TopArtists");
        return apiResult.AsExpectedType;
    }

    public async Task<LastFmTopTracksResponse> GetLastFmTopTracksAsync()
    {
        var apiResult = await Get<LastFmTopTracksResponse>("Life/TopTracks");
        return apiResult.AsExpectedType;
    }
}
