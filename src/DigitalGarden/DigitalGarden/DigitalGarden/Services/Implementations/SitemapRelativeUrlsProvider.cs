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
            "/",
            "/beacons",
            "/life-log",
            "/life-log/mission",
            "/life-log/09-09-2025",
            "/life-log/journeys"
        ];
    }
}
