using DigitalGarden.Client.Services;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Helpers;
using System.Net.Http.Json;

namespace DigitalGarden.Client.MessageHandlers;

public class TokenHandler : DelegatingHandler
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly TokenCache _tokenCache;

    public TokenHandler(IHttpClientFactory clientFactory, TokenCache tokenCache)
    {
        _clientFactory = clientFactory;
        _tokenCache = tokenCache;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_tokenCache.NeedsRefresh)
        {
            await RefreshTokensAsync();
        }

        request.Headers.Add(ApiConstants.AntiforgeryTokenHeader, _tokenCache.AntiforgeryToken);
        request.Headers.Add(ApiConstants.SessionTokenHeader, _tokenCache.SessionToken);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task RefreshTokensAsync()
    {
        var client = _clientFactory.CreateClient(ApiConstants.TokenClientName);
        HttpClientHelper.AddDefaultRequestHeaders(client);

        var antiforgeryToken = await FetchTokenAsync(client, "/antiforgery/token");
        var sessionToken = await FetchTokenAsync(client, "/session/token");

        _tokenCache.Store(antiforgeryToken, sessionToken);
    }

    private static async Task<string> FetchTokenAsync(HttpClient client, string endpoint)
    {
        var response = await client.GetFromJsonAsync<TokenResponse>(endpoint)
            ?? throw new InvalidOperationException($"Could not fetch token from {endpoint}");

        return response.Token;
    }

    internal class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
