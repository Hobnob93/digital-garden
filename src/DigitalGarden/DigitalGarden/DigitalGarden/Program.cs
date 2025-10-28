using DigitalGarden.Components;
using DigitalGarden.Services.Implementations;
using DigitalGarden.Services.Interfaces;
using Serilog;
using Serilog.Events;

try
{
    var builder = WebApplication.CreateBuilder(args);
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
    builder.Host.UseSerilog();

    Log.Information("Setting up services");
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents()
        .AddInteractiveWebAssemblyComponents();

    builder.Services.AddControllers();
    builder.Services.AddTransient<ISitemapRelativeUrlsProvider, SitemapRelativeUrlsProvider>();

    Log.Information("Building app");
    var app = builder.Build();

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
            diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };
    });

    app.UseHttpsRedirection();
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
