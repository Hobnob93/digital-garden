using DigitalGarden.Data;
using DigitalGarden.Data.Dtos;
using DigitalGarden.Extensions;
using DigitalGarden.Services.Interfaces;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Models.Data.Requests;
using DigitalGarden.Shared.Models.Data.Responses;
using DigitalGarden.Shared.Models.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DigitalGarden.Services.Implementations;

public class TraktIngester : IShowIngester
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TraktOptions _traktOptions;
    private readonly ILogger<TraktIngester> _logger;

    public TraktIngester(IHttpClientFactory httpClientFactory, IOptions<TraktOptions> traktOptions, ILogger<TraktIngester> logger)
    {
        _httpClientFactory = httpClientFactory;
        _traktOptions = traktOptions.Value;
        _logger = logger;
    }

    public async Task RunIngestAsync(ApplicationDbContext dbContext, DateTime ingestTimeUtc, CancellationToken cancellationToken)
    {
        var currentAuthState = await dbContext.TraktAuthStates
            .OrderByDescending(s => s.Id)
            .FirstAsync(cancellationToken);

        if (currentAuthState.ExpiresAtUtc - DateTime.UtcNow < TimeSpan.FromDays(_traktOptions.RefreshTokensWithXDaysLeft))
        {
            currentAuthState = await RefreshAuthStateAsync(dbContext, currentAuthState, cancellationToken);
        }

        var watchedMoviesResponse = await _httpClientFactory.GetBearerResponseAsync<TraktWatchedMovieRoot[]>(ApiConstants.TraktClientName, _traktOptions.Endpoints.GetWatchedMovies, currentAuthState.AccessToken, cancellationToken);

        foreach (var watchedMovie in watchedMoviesResponse)
        {
            try
            {
                await HandleWatchedMovieAsync(dbContext, watchedMovie, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update the watched movie '{MovieTitle}'", watchedMovie.Movie.Title);
            }
        }

        var watchedShowsResponse = await _httpClientFactory.GetBearerResponseAsync<TraktWatchedShowRoot[]>(ApiConstants.TraktClientName, _traktOptions.Endpoints.GetWatchedShows, currentAuthState.AccessToken, cancellationToken);

        foreach (var watchedShow in watchedShowsResponse)
        {
            try
            {
                await HandleWatchedShowAsync(dbContext, watchedShow, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update the watched show '{ShowTitle}'", watchedShow.Show.Title);
            }
        }
    }

    private async Task HandleWatchedMovieAsync(ApplicationDbContext dbContext, TraktWatchedMovieRoot watchedMovieRoot, CancellationToken cancellationToken)
    {
        var movieIds = watchedMovieRoot.Movie.Ids;

        var movie = await dbContext.TraktMovies.FindAsync([movieIds.Trakt], cancellationToken);
        var isNewMovie = false;
        if (movie is null)
        {
            isNewMovie = true;
            movie = new TraktMovieDto
            {
                TraktId = movieIds.Trakt,
                ImdbId = movieIds.Imdb,
                Slug = movieIds.Slug,
                TmdbId = movieIds.Tmdb
            };
        }

        movie.Title = watchedMovieRoot.Movie.Title;
        movie.ReleaseYear = watchedMovieRoot.Movie.ReleaseYear;
        movie.LastWatchedUtc = watchedMovieRoot.LastWatchedAtUtc;
        movie.PlayCount = watchedMovieRoot.Plays;

        if (isNewMovie)
        {
            dbContext.TraktMovies.Add(movie);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleWatchedShowAsync(ApplicationDbContext dbContext, TraktWatchedShowRoot watchedShowRoot, CancellationToken cancellationToken)
    {
        var showIds = watchedShowRoot.Show.Ids;

        var show = await dbContext.TraktShows.FindAsync([showIds.Trakt], cancellationToken);
        var isNewShow = false;
        if (show is null)
        {
            isNewShow = true;
            show = new TraktShowDto
            {
                TraktId = showIds.Trakt,
                ImdbId = showIds.Imdb,
                Slug = showIds.Slug,
                TmdbId = showIds.Tmdb,
                TvdbId = showIds.Tvdb,
            };
        }

        show.Title = watchedShowRoot.Show.Title;
        show.ReleaseYear = watchedShowRoot.Show.ReleaseYear;
        show.LastWatchedUtc = watchedShowRoot.LastWatchedAtUtc;

        show.SeasonsWatched = watchedShowRoot.Seasons
            .Select(s => s.SeasonNumber)
            .DefaultIfEmpty(0)
            .Max();

        show.PlayCount = watchedShowRoot.Seasons
            .SelectMany(s => s.Episodes)
            .Select(e => e.Plays)
            .DefaultIfEmpty(0)
            .Max();

        if (isNewShow)
        {
            dbContext.TraktShows.Add(show);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<TraktAuthStateDto> RefreshAuthStateAsync(ApplicationDbContext dbContext, TraktAuthStateDto currentAuthState, CancellationToken cancellationToken)
    {
        var requestData = new RefreshTraktTokens(currentAuthState.RefreshToken, _traktOptions.ClientId, _traktOptions.ApiKey);
        var response = await _httpClientFactory.PostAndGetResponseAsync<RefreshTraktTokens, RefreshedTraktTokens>(ApiConstants.TraktClientName, _traktOptions.Endpoints.GetNewTokens, requestData, cancellationToken);

        var newAuthState = new TraktAuthStateDto
        {
            RefreshToken = response.RefreshToken,
            AccessToken = response.AccessToken,
            FetchedAtUtc = response.CreatedAtUtc,
            ExpiresAtUtc = response.ExpiresAtUtc
        };

        dbContext.TraktAuthStates.Add(newAuthState);
        await dbContext.SaveChangesAsync(cancellationToken);

        return newAuthState;
    }
}
