using System.Net.Http.Headers;

namespace DigitalGarden.Extensions;

public static class HttpClientExtensions
{
    public static async Task<TResponse> GetResponseAsync<TResponse>(this IHttpClientFactory clientFactory, string clientName, string endpoint, CancellationToken cancellationToken = default)
    {
        using var client = clientFactory.CreateClient(clientName);

        var response = await client.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken)
            ?? throw new InvalidDataException($"GET response for endpoint '{endpoint}' could not be parsed!");
    }

    public static async Task<TResponse> GetBearerResponseAsync<TResponse>(this IHttpClientFactory clientFactory, string clientName, string endpoint, string bearerToken, CancellationToken cancellationToken = default)
    {
        using var client = clientFactory.CreateClient(clientName);

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken)
            ?? throw new InvalidDataException($"GET response for endpoint '{endpoint}' could not be parsed!");
    }

    public static async Task<TResponse> PostAndGetResponseAsync<TData, TResponse>(this IHttpClientFactory clientFactory, string clientName, string endpoint, TData data, CancellationToken cancellationToken = default)
    {
        using var client = clientFactory.CreateClient(clientName);

        var response = await client.PostAsJsonAsync(endpoint, data, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken)
            ?? throw new InvalidDataException($"POST response for endpoint '{endpoint}' could not be parsed!");
    }
}
