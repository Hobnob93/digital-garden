using Microsoft.EntityFrameworkCore;

namespace DigitalGarden.Data;

public class ContentSyncService
{
    private readonly IHostEnvironment _hostEnv;
    private readonly ApplicationDbContext _dbContext;

    public ContentSyncService(IHostEnvironment hostEnv, ApplicationDbContext dbContext)
    {
        _hostEnv = hostEnv;
        _dbContext = dbContext;
    }

    public async Task SyncAsync()
    {
        await _dbContext.Database.MigrateAsync();

        await _dbContext.SaveChangesAsync();
    }
}
