using DigitalGarden.Data.Sync;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Models.Options;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.Security.Cryptography;

namespace DigitalGarden.Extensions;

public static class WebApplicationExtensions
{
    public static void UseWorkInProgressMiddleware(this WebApplication app)
    {
        app.Use(async (httpContext, nextDelegate) =>
        {
            var flagOptions = httpContext.RequestServices.GetRequiredService<IOptionsMonitor<GeneralFlagOptions>>().CurrentValue;
            var accessedPath = httpContext.Request.Path.Value?.ToLowerInvariant() ?? "/";
            var pathIsWipPage = accessedPath.StartsWith("/wip");

            if (!flagOptions.IsWip && pathIsWipPage)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

                return;
            }
            else if (flagOptions.IsWip)
            {
                var isGiftsAccess = accessedPath.StartsWith("/xmas");
                var isApiCall = accessedPath.StartsWith("/api");
                var pathIsStaticFile = accessedPath.IsStaticFile();

                if (!pathIsWipPage && !pathIsStaticFile && !isApiCall && !isGiftsAccess)
                {
                    httpContext.Response.Redirect("/wip", permanent: false);

                    return;
                }
            }

            await nextDelegate();
        });
    }

    public static async Task SyncDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var syncService = scope.ServiceProvider.GetRequiredService<ContentSyncService>();

        Log.Information("Synchronising DB Data...");
        await syncService.SyncAsync(CancellationToken.None);
    }

    public static void UseRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (_, _, ex) => ex is null ? LogEventLevel.Information : LogEventLevel.Error;
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
            };
        });
    }

    public static void UseSecurity(this WebApplication app)
    {
        app.UseAntiforgery();
        app.UseCors(ApiConstants.BlazorClientCorsPolicyName);

        app.MapGet("/antiforgery/token", (IAntiforgery antiforgery, HttpContext context) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);

            return Results.Ok(new { token = tokens.RequestToken });
        }).DisableAntiforgery();

        app.MapGet("/session/token", (HttpContext context) =>
        {
            var token = context.Session.GetString(ApiConstants.SessionTokenName);
            if (token is null)
            {
                token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                context.Session.SetString(ApiConstants.SessionTokenName, token);
            }

            return Results.Ok(new { token });
        }).DisableAntiforgery();

        app.UseSession();
    }

    private static bool IsStaticFile(this string path)
    {
        return Path.HasExtension(path)
            || path.StartsWith("/_framework")
            || path.StartsWith("/_blazor")
            || path.StartsWith("/_content")
            || path.StartsWith("/css")
            || path.StartsWith("/js")
            || path.StartsWith("/images")
            || path.StartsWith("/favicon")
            || path.StartsWith("/lib");
    }
}
