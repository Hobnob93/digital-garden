using Microsoft.AspNetCore.Mvc;

namespace DigitalGarden.Controllers;

[Route("[controller]")]
[ApiController]
public class SeoController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;

    public SeoController(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    [HttpGet("/sitemap.xml")]
    [Produces("application/xml")]
    [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any, NoStore = false)]
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

        var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
            <urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">
                {string.Join(Environment.NewLine, urls.Select(url => $"  <url><loc>{baseUri}{url}</loc></url>"))}
            </urlset>";

        return Content(xml, "application/xml");
    }
}
