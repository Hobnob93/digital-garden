using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;

namespace DigitalGarden.Client.Services.Implementations;

public class LifeDataClient : BaseClient, ILifeDataProvider
{
    public LifeDataClient(IHttpClientFactory httpFactory)
        : base(httpFactory)
    {
    }

    public async Task<FamousQuote> GetQuoteOfTheDay()
    {
        var apiResult = await Get<FamousQuote>("Life/QuoteOfTheDay");
        return apiResult.AsExpectedType;
    }
}
