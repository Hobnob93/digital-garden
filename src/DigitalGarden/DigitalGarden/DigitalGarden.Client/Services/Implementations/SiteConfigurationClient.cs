using DigitalGarden.Shared.Models.Options;
using DigitalGarden.Shared.Services.Interfaces;

namespace DigitalGarden.Client.Services.Implementations;

public class SiteConfigurationClient : BaseClient, ISiteConfigurationProvider
{
    public SiteConfigurationClient(IHttpClientFactory httpFactory)
        : base(httpFactory)
    {
        
    }

    public async Task<GeneralFlagOptions> GetSiteFlagOptionsAsync()
    {
        var apiResult = await Get<GeneralFlagOptions>("Config/GetGeneralFlagOptions");
        return apiResult.AsExpectedType;
    }
}
