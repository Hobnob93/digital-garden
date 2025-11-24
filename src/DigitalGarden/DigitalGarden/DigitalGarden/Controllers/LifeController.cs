using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGarden.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LifeController : ControllerBase
{
    private readonly ILifeDataProvider _lifeDataProvider;

    public LifeController(ILifeDataProvider lifeDataProvider)
    {
        _lifeDataProvider = lifeDataProvider;
    }

    [HttpGet(Name = nameof(QuoteOfTheDay))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FamousQuote))]
    public async Task<IActionResult> QuoteOfTheDay()
    {
        var result = await _lifeDataProvider.GetQuoteOfTheDay();

        return Ok(result);
    }
}
