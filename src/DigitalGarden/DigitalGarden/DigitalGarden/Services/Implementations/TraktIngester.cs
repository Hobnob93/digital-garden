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

    public TraktIngester(IHttpClientFactory httpClientFactory, IOptions<TraktOptions> traktOptions)
    {
        _httpClientFactory = httpClientFactory;
        _traktOptions = traktOptions.Value;
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
