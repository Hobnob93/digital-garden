using System.Text.Json;

namespace DigitalGarden.Data.Sync;

public interface ISyncContent
{
    Task SynchronizeAsync(JsonSerializerOptions serializerOptions, string contentRootPath, CancellationToken cancellationToken);
}
