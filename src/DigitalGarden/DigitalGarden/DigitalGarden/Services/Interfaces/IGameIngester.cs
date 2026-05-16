using DigitalGarden.Data;

namespace DigitalGarden.Services.Interfaces;

public interface IGameIngester
{
    Task<bool> ShouldDoFullFetch(ApplicationDbContext dbContext, CancellationToken cancellationToken);
    Task RunIngestAsync(ApplicationDbContext dbContext, DateTime ingestTimeUtc, bool doFullFetch, CancellationToken cancellationToken);
}
