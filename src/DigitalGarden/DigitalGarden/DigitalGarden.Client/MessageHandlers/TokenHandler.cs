using DigitalGarden.Shared.Constants;
using System.Net.Http.Json;

namespace DigitalGarden.Client.MessageHandlers;

public class TokenHandler : DelegatingHandler
{
    private readonly HttpClient _httpClient;
    private string? _cachedAntiforgeryToken;
    private string? _cachedSessionToken;

    public TokenHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ApiConstants.TokenClient);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _cachedAntiforgeryToken ??= await FetchAntiforgeryTokenAsync();
        request.Headers.Add(ApiConstants.AntiforgeryTokenHeader, _cachedAntiforgeryToken);

        _cachedSessionToken ??= await FetchSessionTokenAsync();
        request.Headers.Add(ApiConstants.SessionTokenHeader, _cachedSessionToken);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> FetchAntiforgeryTokenAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<TokenResponse>("/antiforgery/token")
            ?? throw new InvalidOperationException("Could not fetch/deserialize antiforgery token!");

        return response.Token;
    }

    private async Task<string> FetchSessionTokenAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<TokenResponse>("/session/token")
            ?? throw new InvalidOperationException("Could not fetch/deserialize session token!");

        return response.Token;
    }

    internal class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
