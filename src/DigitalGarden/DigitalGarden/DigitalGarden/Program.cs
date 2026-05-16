using DigitalGarden.Components;
using DigitalGarden.Extensions;
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
        .ConfigureHttpClients()
        .ConfigureSecurity(configuration);

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
    app.UseSecurityHeadersMiddleware();

    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
        app.UseExceptionHandler("/Error");
    }
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
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
    await app.RunAsync();

    Log.Information("App finished running!");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed");

    throw;
}
finally
{
    Log.CloseAndFlush();
}
