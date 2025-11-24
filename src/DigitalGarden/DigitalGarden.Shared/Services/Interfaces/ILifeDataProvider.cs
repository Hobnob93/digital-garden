using DigitalGarden.Shared.Models.Data;

namespace DigitalGarden.Shared.Services.Interfaces;

public interface ILifeDataProvider
{
    Task<FamousQuote> GetQuoteOfTheDay();
}
