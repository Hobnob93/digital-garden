using DigitalGarden.Components;
using DigitalGarden.Extensions;
using DigitalGarden.Services.Background;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Helpers;
using DigitalGarden.Shared.Models.Options;
using Microsoft.Extensions.Options;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;
    var configuration = builder.Configuration;
    var isSyncContent = args.Contains("sync-content", StringComparer.OrdinalIgnoreCase);

    services
        .SetupLogging(configuration, builder.Host)
        .AddInteractiveAutoBlazorWithControllers()
        .ConfigureApplication(configuration)
        .AddInternalDependencies(isSyncContent)
        .ConfigureSecurity(configuration);

    services.AddHostedService<DailyIngestService>();

    services.AddHttpClient(ApiConstants.LastFmClientName,
        (sp, client) =>
        {
            var lastFmOptions = sp.GetRequiredService<IOptions<LastFmOptions>>().Value;
            client.BaseAddress = new Uri(lastFmOptions.BaseAddress);
            HttpClientHelper.AddDefaultRequestHeaders(client);
        });

    Log.Information("Building app");
    var app = builder.Build();

    // Sync content when requested and exit application
    if (isSyncContent)
    {
        await app.SyncDataAsync();

        Log.Information("Sync done! Exiting...");
        return;
    }

    Log.Information("Configuring HTTP pipeline");
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
        app.UseExceptionHandler("/Error");
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseRequestLogging();
    app.UseHttpsRedirection();
    app.MapStaticAssets();
    app.UseStaticFiles();

    app.UseSecurity();
    app.MapControllers();

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .AddInteractiveWebAssemblyRenderMode()
        .AddAdditionalAssemblies(typeof(DigitalGarden.Client.Components._Imports).Assembly)
        .AddAdditionalAssemblies(typeof(DigitalGarden.Shared.Components._Imports).Assembly);

    Log.Information("Running app...");
    app.Run();

    Log.Information("App finished running!");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed");
}
finally
{
    Log.CloseAndFlush();
}
