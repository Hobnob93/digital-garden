using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalGarden.Tests.Helpers;

public static class ControllerTestHelper
{
    public static TController CreateController<TController>(
        Action<IServiceCollection>? configureServices = null,
        string siteAddress = "https://example.com")
        where TController : ControllerBase
    {
        var uri = new Uri(siteAddress);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = uri.Scheme;
        httpContext.Request.Host = new HostString(uri.Host);

        var services = new ServiceCollection();
        configureServices?.Invoke(services);
        var serviceProvider = services.BuildServiceProvider();

        var controller = ActivatorUtilities.CreateInstance<TController>(serviceProvider);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        return controller;
    }

    public static string? GetResponseEntityTag(this ControllerBase controller)
    {
        return controller.Response.Headers.ETag.FirstOrDefault();
    }
}
