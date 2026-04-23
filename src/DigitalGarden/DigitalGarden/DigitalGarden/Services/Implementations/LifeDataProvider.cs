using DigitalGarden.Data;
using DigitalGarden.Data.Dtos;
using DigitalGarden.Extensions;
using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalGarden.Services.Implementations;

public class LifeDataProvider : ILifeDataProvider
{
    private readonly ApplicationDbContext _dbContext;

    public LifeDataProvider(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public async Task<LastFmTopArtistsResponse> GetLastFmTopArtistsAsync()
    {
        var latest = await GetLatestMusicSnapshotOrDefault();

        List<LastFmArtist> artists = [];
        if (latest != null)
        {
            foreach (var artist in latest.TopArtists)
            {
                artists.Add(new LastFmArtist
                (
                    artist.Name,
                    artist.PlayCount
                ));
            }
        }

        return new LastFmTopArtistsResponse(new LastFmTopArtists
        (
            artists.ToArray()
        ));
    }

    public async Task<LastFmTopTracksResponse> GetLastFmTopTracksAsync()
    {
        var latest = await GetLatestMusicSnapshotOrDefault();

        List<LastFmTrack> tracks = [];
        if (latest != null)
        {
            foreach (var track in latest.TopTracks)
            {
                tracks.Add(new LastFmTrack
                (
                    track.Name,
                    new LastFmTrackArtist(track.ArtistName),
                    track.PlayCount
                ));
            }
        }

        return new LastFmTopTracksResponse(new LastFmTopTracks
        (
            tracks.ToArray()
        ));
    }

    private async Task<LastFmSnapshotDto?> GetLatestMusicSnapshotOrDefault()
    {
        return await _dbContext.LastFmSnapshots
            .Include(s => s.TopArtists.OrderBy(a => a.Rank))
            .Include(s => s.TopTracks.OrderBy(t => t.Rank))
            .OrderByDescending(s => s.CapturedAtUtc)
            .FirstOrDefaultAsync();
    }
}
