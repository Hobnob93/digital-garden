using System.Text.Json;

namespace DigitalGarden.Data.Sync;

public class SyncBeaconsContent : ISyncContent
{
    public async Task Synchronize(JsonSerializerOptions serializerOptions, string contentRootPath)
    {
        var beaconsFilePath = $"{contentRootPath}/Beacons.json";
        if (!File.Exists(beaconsFilePath))
            return;

        var json = await File.ReadAllTextAsync(beaconsFilePath);

        var categorySlugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}
