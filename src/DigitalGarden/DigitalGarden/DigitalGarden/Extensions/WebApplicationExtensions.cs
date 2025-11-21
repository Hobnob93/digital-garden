using DigitalGarden.Shared.Models.Options;
using Microsoft.Extensions.Options;

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
