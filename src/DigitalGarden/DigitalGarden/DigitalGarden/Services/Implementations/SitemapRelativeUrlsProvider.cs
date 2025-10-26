using DigitalGarden.Services.Interfaces;

namespace DigitalGarden.Services.Implementations;

public class SitemapRelativeUrlsProvider : ISitemapRelativeUrlsProvider
{
    public string[] GetPublicRelativeUrls()
    {
        // TODO: supply from config
        // TODO: fetch digital garden content slugs from db to add here, too
        return
        [
            "/"
        ];
    }
}
