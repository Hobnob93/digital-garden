using DigitalGarden.Data;
using DigitalGarden.Data.Dtos;
using DigitalGarden.Services.Interfaces;
using DigitalGarden.Shared.Models.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DigitalGarden.Services.Background;

public class DailyIngestService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DailyIngestOptions _options;
    private readonly ILogger<DailyIngestService> _logger;

    public DailyIngestService(IServiceScopeFactory scopeFactory, IOptions<DailyIngestOptions> options, ILogger<DailyIngestService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Allow for general application setup before running background process
        await Task.Delay(TimeSpan.FromMinutes(_options.MinimumDelayMinutes), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = await TimeUntilNextAsync(stoppingToken);
            _logger.LogInformation("Next ingest in {Delay}", delay);

            try
            {
                await Task.Delay(delay, stoppingToken);
                await RunIngestAsync(stoppingToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ingest failed; will retry on next cycle!");
            }
        }
    }

    private async Task RunIngestAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var dailySnapshot = new DailyIngestSnapshotDto
        {
            CapturedAtUtc = DateTime.UtcNow
        };

        //await RunMusicIngestionAsync(scope, dbContext, dailySnapshot, stoppingToken);
        await RunGameIngestionAsync(scope, dbContext, dailySnapshot, stoppingToken);

        dbContext.DailySnapshots.Add(dailySnapshot);
        await dbContext.SaveChangesAsync(stoppingToken);

        _logger.LogInformation("Daily ingestion ran at {DateTimeUtc}", dailySnapshot.CapturedAtUtc);
    }

    private async Task RunMusicIngestionAsync(IServiceScope scope, ApplicationDbContext dbContext, DailyIngestSnapshotDto snapshot, CancellationToken stoppingToken)
    {
        try
        {
            var musicIngester = scope.ServiceProvider.GetRequiredService<IMusicIngester>();
            await musicIngester.RunIngestAsync(dbContext, snapshot.CapturedAtUtc, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run music ingestion!");
        }
    }

    private async Task RunGameIngestionAsync(IServiceScope scope, ApplicationDbContext dbContext, DailyIngestSnapshotDto snapshot, CancellationToken stoppingToken)
    {
        try
        {
            var gameIngester = scope.ServiceProvider.GetRequiredService<IGameIngester>();
            var shouldDoFullGameRun = await gameIngester.ShouldDoFullFetch(dbContext, stoppingToken);
            await gameIngester.RunIngestAsync(dbContext, snapshot.CapturedAtUtc, shouldDoFullGameRun, stoppingToken);

            if (shouldDoFullGameRun)
            {
                snapshot.FullGameFetchAtUtc = DateTime.UtcNow;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run game ingestion!");
        }
    }

    private async Task<TimeSpan> TimeUntilNextAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var lastRunUtc = await dbContext.DailySnapshots
            .OrderByDescending(s => s.CapturedAtUtc)
            .Select(s => (DateTime?)s.CapturedAtUtc)
            .FirstOrDefaultAsync(stoppingToken);

        var minimumDelay = TimeSpan.FromMinutes(_options.MinimumDelayMinutes);

        if (lastRunUtc is null)
            return minimumDelay; // first ever run

        var target = lastRunUtc.Value.AddDays(_options.DelayInDays);
        var delay = target - DateTime.UtcNow;

        return delay < minimumDelay ? minimumDelay : delay;
    }
}
