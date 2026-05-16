using DigitalGarden.Data;

namespace DigitalGarden.Services.Interfaces;

public interface IShowIngester
{
    Task RunIngestAsync(ApplicationDbContext dbContext, DateTime ingestTimeUtc, CancellationToken cancellationToken);
}
