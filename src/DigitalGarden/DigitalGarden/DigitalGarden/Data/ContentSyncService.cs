using Microsoft.EntityFrameworkCore;

namespace DigitalGarden.Data;

public class ContentSyncService
{
    private readonly ApplicationDbContext _dbContext;

    public ContentSyncService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SyncAsync()
    {
        await _dbContext.Database.MigrateAsync();

        await _dbContext.SaveChangesAsync();
    }
}
