using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGarden.Controllers;

[Route("api/[controller]")]
[ApiController]
[ValidateAntiForgeryToken]
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
        var result = await _lifeDataProvider.GetQuoteOfTheDayAsync();

        return Ok(result);
    }

    [HttpGet(Name = nameof(RecentLifeLogs))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecentLifeLog[]))]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> RecentLifeLogs()
    {
        var result = await _lifeDataProvider.GetRecentLifeLogsAsync();

        return Ok(result);
    }

    [HttpGet(Name = nameof(TopArtists))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LastFmTopArtistsResponse))]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> TopArtists()
    {
        var result = await _lifeDataProvider.GetLastFmTopArtists();

        return Ok(result);
    }
}
