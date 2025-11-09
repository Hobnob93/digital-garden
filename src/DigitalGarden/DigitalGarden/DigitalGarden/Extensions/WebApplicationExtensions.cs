using DigitalGarden.Data;
using DigitalGarden.Shared.Models.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

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

    public static async Task InitializeDbMigrationAndSeeding(this WebApplication app)
    {
        Log.Information("DB Migrations and seeding...");

        var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        await dbContext.SaveChangesAsync();
    }

    private static bool IsStaticFile(this string path)
    {
        return Path.HasExtension(path)
            || path.StartsWith("/_framework")
            || path.StartsWith("/_content")
            || path.StartsWith("/css")
            || path.StartsWith("/js")
            || path.StartsWith("/images")
            || path.StartsWith("/favicon")
            || path.StartsWith("/lib");
    }
}
