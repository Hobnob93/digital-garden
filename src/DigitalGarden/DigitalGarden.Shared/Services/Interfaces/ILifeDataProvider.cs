using DigitalGarden.Shared.Models.Data;

namespace DigitalGarden.Shared.Services.Interfaces;

public interface ILifeDataProvider
{
    Task<FamousQuote> GetQuoteOfTheDayAsync();
    Task<RecentLifeLog[]> GetRecentLifeLogsAsync();
    Task<LastFmTopArtistsResponse> GetLastFmTopArtistsAsync();
    Task<LastFmTopTracksResponse> GetLastFmTopTracksAsync();
}
