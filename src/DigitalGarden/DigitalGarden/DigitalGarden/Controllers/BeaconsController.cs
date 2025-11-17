using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGarden.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BeaconsController : ControllerBase
{
    private readonly IBeaconProvider _beaconProvider;

    public BeaconsController(IBeaconProvider beaconProvider)
    {
        _beaconProvider = beaconProvider;
    }

    [HttpGet(Name = nameof(GetAllWithinCategories))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ICollection<BeaconCategoryItems>))]
    public async Task<IActionResult> GetAllWithinCategories()
    {
        var result = await _beaconProvider.GetAllItems();

        return Ok(result);
    }
}
