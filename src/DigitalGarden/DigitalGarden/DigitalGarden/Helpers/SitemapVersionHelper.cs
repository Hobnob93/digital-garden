using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace DigitalGarden.Helpers;

public static class SitemapVersionHelper
{
    // TODO: Change tuple to be URL data points once they can be fetched

    /// <summary>
    /// Uses incoming URLs and their last update time to produce a "stable" hex SHA-256 for sitemap caching by crawlers
    /// </summary>
    /// <param name="linkData">A colleciton of fully qualified URLs with their most recent update time, if available</param>
    /// <returns>A "stable" hex SHA-256 based on the url-ordered lines in the format 'url|ticks'</returns>
    public static string Compute(IEnumerable<(string Url, DateTimeOffset? LastModified)> linkData)
    {
        using var sha = SHA256.Create();

        var orderedLinkData = linkData
            .Select(l => (Url: l.Url.Trim().ToLowerInvariant(), Ticks: l.LastModified?.UtcDateTime.Ticks ?? 0L))
            .OrderBy(l => l.Url, StringComparer.Ordinal);

        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), bufferSize: 1024, leaveOpen: true);

        // Ensures new lines are deterministic (ignores OS preference)
        streamWriter.NewLine = "\n";      
        
        foreach (var (url, ticks) in orderedLinkData)
        {
            // Ensures invariant culture used for ticks output; important for determinism
            var invariantTicks = ticks.ToString(CultureInfo.InvariantCulture);

            streamWriter.WriteLine($"{url}|{invariantTicks}");
        }

        memoryStream.Position = 0;

        var hash = sha.ComputeHash(memoryStream);
        return Convert.ToHexString(hash);
    }
}
