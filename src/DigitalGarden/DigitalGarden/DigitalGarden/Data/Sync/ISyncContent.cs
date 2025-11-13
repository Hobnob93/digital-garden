using System.Text.Json;

namespace DigitalGarden.Data.Sync;

public interface ISyncContent
{
    Task Synchronize(JsonSerializerOptions serializerOptions, string contentRootPath);
}
