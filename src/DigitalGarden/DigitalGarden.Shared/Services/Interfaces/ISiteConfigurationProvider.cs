using DigitalGarden.Shared.Models.Options;

namespace DigitalGarden.Shared.Services.Interfaces;

public interface ISiteConfigurationProvider
{
    Task<GeneralFlagOptions> GetSiteFlagOptionsAsync();
}
