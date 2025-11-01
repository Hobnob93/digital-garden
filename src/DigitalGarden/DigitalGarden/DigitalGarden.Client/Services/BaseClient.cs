using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Models.Client;
using OneOf.Types;
using System.Net;
using System.Net.Http.Json;

namespace DigitalGarden.Client.Services;

public abstract class BaseClient
{
    private readonly IHttpClientFactory _httpFactory;

    protected BaseClient(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    protected async Task<ApiClientResult<TResult>> Get<TResult>(string endpoint)
    {
        HttpResponseMessage? result = null;
        try
        {
            result = await CreateClient().GetAsync(endpoint);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<TResult>()
                ?? throw new InvalidDataException($"GET result for endpoint '{endpoint}' could not be parsed!");
        }
        catch (Exception ex)
        {
            return await ExceptionToApiClientResult<TResult>(ex, result);
        }
    }

    protected async Task<ApiClientResult<string>> Get(string endpoint)
    {
        HttpResponseMessage? result = null;
        try
        {
            result = await CreateClient().GetAsync(endpoint);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return await ExceptionToApiClientResult<string>(ex, result);
        }
    }

    protected HttpClient CreateClient()
    {
        var apiClientName = ApiClientNames.AnonymousClientName;
        return _httpFactory.CreateClient(apiClientName);
    }

    protected static async Task<ApiClientResult<T>> ExceptionToApiClientResult<T>(Exception ex, HttpResponseMessage? result)
    {
        string resultContent = "Sorry, but something went wrong. Please try again later.";
        try
        {
            if (result is not null)
            {
                var content = await result.Content.ReadAsStringAsync();
                resultContent = string.IsNullOrWhiteSpace(content) ? resultContent : content;
            }
        }
        catch
        {
            resultContent = ex.Message;
        }

        return ex switch
        {
            HttpRequestException httpEx when httpEx.StatusCode == HttpStatusCode.NotFound => new NotFound(),
            HttpRequestException httpEx when httpEx.StatusCode == HttpStatusCode.BadRequest => new Error<string>(resultContent),
            _ => new Error<string>(ex.Message)
        };
    }
}
