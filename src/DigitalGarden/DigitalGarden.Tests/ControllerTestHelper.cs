using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGarden.Tests;

public static class ControllerTestHelper
{
    public static TController CreateController<TController>(string siteAddress = "https://example.com") where TController : ControllerBase, new()
    {
        var uri = new Uri(siteAddress);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = uri.Scheme;
        httpContext.Request.Host = new HostString(uri.Host);

        var controller = new TController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        return controller;
    }

    public static string? GetResponseEntityTag(this ControllerBase controller)
    {
        return controller.Response.Headers.ETag.FirstOrDefault();
    }
}
