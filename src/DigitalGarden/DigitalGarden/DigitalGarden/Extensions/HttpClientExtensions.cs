namespace DigitalGarden.Extensions;

public static class HttpClientExtensions
{
    public static async Task<T> GetDataUsingClientAsync<T>(this IHttpClientFactory clientFactory, string clientName, string endpoint, CancellationToken cancellationToken = default)
    {
        using var client = clientFactory.CreateClient(clientName);

        var result = await client.GetAsync(endpoint, cancellationToken);
        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<T>(cancellationToken)
            ?? throw new InvalidDataException($"GET result for endpoint '{endpoint}' could not be parsed!");
    }
}
