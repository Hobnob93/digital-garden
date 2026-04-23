using DigitalGarden.Data;

namespace DigitalGarden.Services.Interfaces;

public interface IMusicIngester
{
    Task RunIngestAsync(ApplicationDbContext dbContext, DateTime ingestTimeUtc, CancellationToken cancellationToken);
}
