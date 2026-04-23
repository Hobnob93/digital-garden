using DigitalGarden.Data;
using DigitalGarden.Data.Dtos;
using DigitalGarden.Services.Interfaces;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Models.Options;
using Microsoft.Extensions.Options;

namespace DigitalGarden.Services.Implementations;

public class LastFmIngester : IMusicIngester
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly LastFmOptions _lastFmOptions;

    public LastFmIngester(IHttpClientFactory httpClientFactory, IOptions<LastFmOptions> lastFmOptions)
    {
        _httpClientFactory = httpClientFactory;
        _lastFmOptions = lastFmOptions.Value;
    }

    public async Task RunIngestAsync(ApplicationDbContext dbContext, DateTime ingestTimeUtc, CancellationToken cancellationToken)
    {
        var artists = await GetLastFmTopArtistsAsync(cancellationToken);
        var tracks = await GetLastFmTopTracksAsync(cancellationToken);

        var snapshot = new LastFmSnapshotDto
        {
            CapturedAtUtc = ingestTimeUtc,
            TopArtists = artists.TopArtists.Artists
                .Select((a, i) => new TopArtistEntryDto
                {
                    Rank = i + 1,
                    Name = a.Name,
                    PlayCount = a.PlayCount
                })
                .ToList(),
            TopTracks = tracks.TopTracks.Tracks
                .Select((t, i) => new TopTrackEntryDto
                {
                    Rank = i + 1,
                    Name = t.Name,
                    ArtistName = t.Artist.Name,
                    PlayCount = t.PlayCount
                })
                .ToList()
        };

        dbContext.LastFmSnapshots.Add(snapshot);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<LastFmTopArtistsResponse> GetLastFmTopArtistsAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(ApiConstants.LastFmClientName);

        var endpoint = string.Format(_lastFmOptions.TopArtistsEndpoint, _lastFmOptions.UserId, _lastFmOptions.ApiKey);
        var result = await client.GetAsync(endpoint, cancellationToken);
        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<LastFmTopArtistsResponse>(cancellationToken)
            ?? throw new InvalidDataException($"GET result for endpoint '{endpoint}' could not be parsed!");
    }

    private async Task<LastFmTopTracksResponse> GetLastFmTopTracksAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(ApiConstants.LastFmClientName);

        var endpoint = string.Format(_lastFmOptions.TopTracksEndpoint, _lastFmOptions.UserId, _lastFmOptions.ApiKey);
        var result = await client.GetAsync(endpoint, cancellationToken);
        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<LastFmTopTracksResponse>(cancellationToken)
            ?? throw new InvalidDataException($"GET result for endpoint '{endpoint}' could not be parsed!");
    }
}
