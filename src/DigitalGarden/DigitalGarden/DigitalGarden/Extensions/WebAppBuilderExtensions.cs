using DigitalGarden.Services.Implementations;
using DigitalGarden.Services.Interfaces;
using DigitalGarden.Shared.Models.Options;
using DigitalGarden.Shared.Services.Interfaces;
using Serilog;

namespace DigitalGarden.Extensions;

public static class WebAppBuilderExtensions
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

    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Information("Setting up options from configuration");

        services.Configure<GeneralFlagOptions>(configuration.GetSection(GeneralFlagOptions.SectionName));

        return services;
    }

    public static IServiceCollection AddInternalDependencies(this IServiceCollection services)
    {
        Log.Information("Adding internal dependencies to DI");

        services.AddTransient<ISitemapRelativeUrlsProvider, SitemapRelativeUrlsProvider>();
        services.AddTransient<ISiteConfigurationProvider, SiteConfigurationProvider>();

        return services;
    }
}
