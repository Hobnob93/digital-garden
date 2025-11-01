using DigitalGarden.Shared.Models.Options;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DigitalGarden.Services.Implementations;

public class SiteConfigurationProvider : ISiteConfigurationProvider
{
    private readonly IOptionsMonitor<GeneralFlagOptions> _flagOptionsMonitor;

    public SiteConfigurationProvider(IOptionsMonitor<GeneralFlagOptions> flagOptionsMonitor)
    {
        _flagOptionsMonitor = flagOptionsMonitor;
    }

    public GeneralFlagOptions GetSiteFlagOptions()
    {
        return _flagOptionsMonitor.CurrentValue;
    }
}
