using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DigitalGarden.Data.Sync;

public class ContentSyncService
{
    private readonly IHostEnvironment _hostEnv;
    private readonly ApplicationDbContext _dbContext;
    private readonly IEnumerable<ISyncContent> _contentSyncs;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };


    public ContentSyncService(IHostEnvironment hostEnv, ApplicationDbContext dbContext, IEnumerable<ISyncContent> contentSyncs)
    {
        _hostEnv = hostEnv;
        _dbContext = dbContext;
        _contentSyncs = contentSyncs;
    }

    public async Task SyncAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);

        var contentRootPath = Path.Combine(_hostEnv.ContentRootPath, "Data", "_content");
        foreach (var contentSync in _contentSyncs)
            await contentSync.SynchronizeAsync(_serializerOptions, contentRootPath, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
