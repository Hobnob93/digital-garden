using DigitalGarden.Data;
using DigitalGarden.Extensions;
using DigitalGarden.Shared.Models.Data;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalGarden.Services.Implementations;

public class LifeDataProvider : ILifeDataProvider
{
    private readonly ApplicationDbContext _dbContext;

    public LifeDataProvider(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FamousQuote> GetQuoteOfTheDay()
    {
        var allQuotes = await _dbContext.FamousQuotes.ToListAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;

        var random = new Random(today);
        var randomIndex = random.Next(allQuotes.Count);

        return allQuotes[randomIndex].ToDomain();
    }
}
