using DigitalGarden.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace DigitalGarden.Controllers;

[Route("[controller]")]
[ApiController]
public class SeoController : ControllerBase
{
    [HttpGet("/sitemap.xml")]
    [Produces("application/xml")]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, NoStore = false)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult Sitemap()
    {
        var request = HttpContext.Request;
        var baseUri = $"{request.Scheme}://{request.Host}";

        // TODO: supply from config
        // TODO: fetch digital garden content slugs from db to add here, too
        var urls = new[]
        {
            "/"
        };

        // Determine sitemap version and set entity tag for the response
        var sitemapVersion = CreateVersionFromUrls(urls);
        var entityTagValue = new EntityTagHeaderValue($"\"{sitemapVersion}\"", isWeak: true);
        Response.Headers.ETag = entityTagValue.ToString();

        // Use the If-None-Match header to determine if the sitemap has been modified since previous request
        var ifNoneMatchHeader = Request.GetTypedHeaders().IfNoneMatch;
        if (ifNoneMatchHeader != null && ifNoneMatchHeader.Count > 0)
        {
            if (ifNoneMatchHeader.Any(v => v.Tag == "*") || ifNoneMatchHeader.Any(v => StringSegment.Equals(v.Tag, entityTagValue.Tag, StringComparison.Ordinal)))
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }
        }

        // Construct and return most up to date XML
        var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
            <urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">
                {string.Join(Environment.NewLine, urls.Select(url => $"  <url><loc>{baseUri}{url}</loc></url>"))}
            </urlset>";

        return Content(xml, "application/xml");
    }

    private static string CreateVersionFromUrls(string[] urls)
    {
        return SitemapVersionHelper.Compute(urls.Select(u => (u, (DateTimeOffset?)null)));
    }
}
