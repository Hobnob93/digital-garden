using DigitalGarden.Data;
using DigitalGarden.Data.Sync;
using DigitalGarden.Services.Background;
using DigitalGarden.Services.Implementations;
using DigitalGarden.Services.Interfaces;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Models.Options;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net.Http.Headers;

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

        services.AddOptionsWithValidateOnStart<SteamOptions>()
            .Bind(configuration.GetSection(SteamOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.UserId), $"{SteamOptions.SectionName}:{nameof(SteamOptions.UserId)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), $"{SteamOptions.SectionName}:{nameof(SteamOptions.ApiKey)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseAddress), $"{SteamOptions.SectionName}:{nameof(SteamOptions.BaseAddress)} is required")
            .Validate(o => o.MaxFullUpdates > 0, $"{SteamOptions.SectionName}:{nameof(SteamOptions.MaxFullUpdates)} must be positive")
            .Validate(o => o.UpdateDelayDays > 0, $"{SteamOptions.SectionName}:{nameof(SteamOptions.UpdateDelayDays)} must be positive")
            .Validate(o => o.FullFetchDelayDays > 0, $"{SteamOptions.SectionName}:{nameof(SteamOptions.FullFetchDelayDays)} must be positive")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Endpoints.GetSchemaForGame), $"{SteamOptions.SectionName}:{nameof(SteamOptions.Endpoints)}:{nameof(SteamOptionsEndpoints.GetSchemaForGame)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Endpoints.GetPlayerAchievements), $"{SteamOptions.SectionName}:{nameof(SteamOptions.Endpoints)}:{nameof(SteamOptionsEndpoints.GetPlayerAchievements)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Endpoints.GetGlobalGameAchievements), $"{SteamOptions.SectionName}:{nameof(SteamOptions.Endpoints)}:{nameof(SteamOptionsEndpoints.GetGlobalGameAchievements)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Endpoints.GetOwnedGames), $"{SteamOptions.SectionName}:{nameof(SteamOptions.Endpoints)}:{nameof(SteamOptionsEndpoints.GetOwnedGames)} is required");

        services.AddOptionsWithValidateOnStart<TraktOptions>()
            .Bind(configuration.GetSection(TraktOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ClientId), $"{TraktOptions.SectionName}:{nameof(TraktOptions.ClientId)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), $"{TraktOptions.SectionName}:{nameof(TraktOptions.ApiKey)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseAddress), $"{TraktOptions.SectionName}:{nameof(TraktOptions.BaseAddress)} is required")
            .Validate(o => o.RefreshTokensWithXDaysLeft > 0, $"{TraktOptions.SectionName}:{nameof(TraktOptions.RefreshTokensWithXDaysLeft)} must be positive")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Endpoints.GetNewTokens), $"{TraktOptions.SectionName}:{nameof(TraktOptions.Endpoints)}:{nameof(TraktOptionsEndpoints.GetNewTokens)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Endpoints.GetWatchedShows), $"{TraktOptions.SectionName}:{nameof(TraktOptions.Endpoints)}:{nameof(TraktOptionsEndpoints.GetWatchedShows)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Endpoints.GetWatchedMovies), $"{TraktOptions.SectionName}:{nameof(TraktOptions.Endpoints)}:{nameof(TraktOptionsEndpoints.GetWatchedMovies)} is required");

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
        services.AddTransient<IGameIngester, SteamIngester>();
        services.AddTransient<IShowIngester, TraktIngester>();

        if (isSyncContent)
        {
            services.AddDataSynchronisation();
        }
        else
        {
            services.AddHostedService<DailyIngestService>();
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

    public static IServiceCollection ConfigureHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient(ApiConstants.LastFmClientName, (sp, client) =>
        {
            var lastFmOptions = sp.GetRequiredService<IOptions<LastFmOptions>>().Value;
            client.BaseAddress = new Uri(lastFmOptions.BaseAddress);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DigitalGarden/1.0");
        });

        services.AddHttpClient(ApiConstants.SteamClientName, (sp, client) =>
        {
            var steamOptions = sp.GetRequiredService<IOptions<SteamOptions>>().Value;
            client.BaseAddress = new Uri(steamOptions.BaseAddress);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DigitalGarden/1.0");
        });

        services.AddHttpClient(ApiConstants.TraktClientName, (sp, client) =>
        {
            var traktOptions = sp.GetRequiredService<IOptions<TraktOptions>>().Value;
            client.BaseAddress = new Uri(traktOptions.BaseAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("trakt-api-version", "2");
            client.DefaultRequestHeaders.Add("trakt-api-key", traktOptions.ClientId);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("DigitalGarden/1.0");
        });

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
