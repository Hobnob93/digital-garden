using DigitalGarden.Shared.Models.Options;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGarden.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ConfigController : ControllerBase
{
    private readonly ISiteConfigurationProvider _siteConfigurationProvider;

    public ConfigController(ISiteConfigurationProvider siteConfigurationProvider)
    {
        _siteConfigurationProvider = siteConfigurationProvider;
    }

    [HttpGet(Name = nameof(GetGeneralFlagOptions))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralFlagOptions))]
    public async Task<IActionResult> GetGeneralFlagOptions()
    {
        var result = await _siteConfigurationProvider.GetSiteFlagOptionsAsync();

        return Ok(result);
    }
}
