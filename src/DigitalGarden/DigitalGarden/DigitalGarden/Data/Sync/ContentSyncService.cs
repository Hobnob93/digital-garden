using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DigitalGarden.Data.Sync;

public class ContentSyncService
{
    private readonly IHostEnvironment _hostEnv;
    private readonly ApplicationDbContext _dbContext;
    private readonly ISyncContent[] _contentSyncs;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };


    public ContentSyncService(IHostEnvironment hostEnv, ApplicationDbContext dbContext, ISyncContent[] contentSyncs)
    {
        _hostEnv = hostEnv;
        _dbContext = dbContext;
        _contentSyncs = contentSyncs;
    }

    public async Task SyncAsync()
    {
        await _dbContext.Database.MigrateAsync();

        var contentRootPath = Path.Combine(_hostEnv.ContentRootPath, "Data", "_content");
        foreach (var contentSync in _contentSyncs)
            await contentSync.Synchronize(_serializerOptions, contentRootPath);

        await _dbContext.SaveChangesAsync();
    }
}
