using DigitalGarden.Data;
using DigitalGarden.Data.Sync;
using DigitalGarden.Services.Implementations;
using DigitalGarden.Services.Interfaces;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Models.Options;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DigitalGarden.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupLogging(this IServiceCollection services, IConfiguration configuration, IHostBuilder host)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        host.UseSerilog();

        return services;
    }

    public static IServiceCollection AddInteractiveAutoBlazorWithControllers(this IServiceCollection services)
    {
        Log.Information("Adding interactive auto blazor components");
        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        services.AddControllers();

        return services;
    }

    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Information("Setting up options from configuration");

        services.Configure<GeneralFlagOptions>(configuration.GetSection(GeneralFlagOptions.SectionName));

        services.Configure<DailyIngestOptions>(configuration.GetSection(DailyIngestOptions.SectionName));

        services.AddOptionsWithValidateOnStart<LastFmOptions>()
            .Bind(configuration.GetSection(LastFmOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Secret), $"{LastFmOptions.SectionName}:{nameof(LastFmOptions.Secret)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), $"{LastFmOptions.SectionName}:{nameof(LastFmOptions.ApiKey)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.UserId), $"{LastFmOptions.SectionName}:{nameof(LastFmOptions.UserId)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseAddress), $"{LastFmOptions.SectionName}:{nameof(LastFmOptions.BaseAddress)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.TopArtistsEndpoint), $"{LastFmOptions.SectionName}:{nameof(LastFmOptions.TopArtistsEndpoint)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.TopTracksEndpoint), $"{LastFmOptions.SectionName}:{nameof(LastFmOptions.TopTracksEndpoint)} is required");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString);
        });

        return services;
    }

    public static IServiceCollection AddInternalDependencies(this IServiceCollection services, bool isSyncContent)
    {
        Log.Information("Adding internal dependencies to DI");

        services.AddTransient<ISitemapRelativeUrlsProvider, SitemapRelativeUrlsProvider>();
        services.AddTransient<ISiteConfigurationProvider, SiteConfigurationProvider>();
        services.AddTransient<IBeaconProvider, BeaconProvider>();
        services.AddTransient<ILifeDataProvider, LifeDataProvider>();
        services.AddTransient<IMusicIngester, LastFmIngester>();

        if (isSyncContent)
        {
            services.AddDataSynchronisation();
        }

        return services;
    }

    public static IServiceCollection AddDataSynchronisation(this IServiceCollection services)
    {
        Log.Information("Adding data sync dependencies to DI");

        services.AddTransient<ContentSyncService>();
        services.AddTransient<ISyncContent, SyncBeaconsContent>();
        services.AddTransient<ISyncContent, SyncFamousQuotesContent>();
        services.AddTransient<ISyncContent, SyncRecentLifeLogsContent>();

        return services;
    }

    public static IServiceCollection ConfigureSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("api", o =>
            {
                o.PermitLimit = 30;
                o.Window = TimeSpan.FromMinutes(1);
            });
        });

        var allowedOrigin = configuration["AllowedOrigin"]
            ?? throw new InvalidOperationException("'AllowedOrigin' coudl not be found in configuration!");

        services.AddCors(options =>
        {
            options.AddPolicy(ApiConstants.BlazorClientCorsPolicyName, policy =>
                policy.WithOrigins(allowedOrigin)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials());
        });

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        return services
            .AddDistributedMemoryCache()
            .AddAntiforgery();
    }
}
