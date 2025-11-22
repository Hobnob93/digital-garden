using DigitalGarden.Components;
using DigitalGarden.Data.Sync;
using DigitalGarden.Extensions;
using Serilog;
using Serilog.Events;

try
{
    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;
    var configuration = builder.Configuration;

    services
        .SetupLogging(configuration, builder.Host)
        .AddInteractiveAutoBlazorWithControllers()
        .ConfigureApplication(configuration)
        .AddInternalDependencies();

    var isSyncContent = args.Contains("sync-content", StringComparer.OrdinalIgnoreCase);
    if (isSyncContent)
    {
        services.AddDataSynchronisation();
    }

    Log.Information("Building app");
    var app = builder.Build();

    // Sync content when requested and exit application
    if (isSyncContent)
    {
        using var scope = app.Services.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<ContentSyncService>();

        Log.Information("Synchronising DB Data...");
        await syncService.SyncAsync(CancellationToken.None);

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

    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (_, _, ex) => ex is null ? LogEventLevel.Information : LogEventLevel.Error;
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };
    });

    app.UseHttpsRedirection();
    app.MapStaticAssets();

    app.UseWorkInProgressMiddleware();
    app.MapControllers();

    app.UseStaticFiles();
    app.UseAntiforgery();

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
