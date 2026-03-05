using DigitalGarden.Shared.Constants;
using System.Net.Http.Json;

namespace DigitalGarden.Client.MessageHandlers;

public class AntiforgeryTokenHandler : DelegatingHandler
{
    private readonly HttpClient _httpClient;
    private string? _cachedToken;

    public AntiforgeryTokenHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(ApiClientNames.AntiforgeryClient);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _cachedToken ??= await FetchTokenAsync();
        request.Headers.Add("X-XSRF-TOKEN", _cachedToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> FetchTokenAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<TokenResponse>("/antiforgery/token");
        return response!.Token;
    }

    internal class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
