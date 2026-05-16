using System.Text.Json.Serialization;

namespace DigitalGarden.Shared.Models.Data.Responses;

public record RefreshedTraktTokens
(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("refresh_token")] string RefreshToken,
    [property: JsonPropertyName("expires_in")] int ExpiresInSeconds,
    [property: JsonPropertyName("created_at")] long CreatedAtSeconds,
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("scope")] string Scope
)
{
    public DateTime CreatedAtUtc => DateTimeOffset.FromUnixTimeSeconds(CreatedAtSeconds).UtcDateTime;
    public DateTime ExpiresAtUtc => DateTimeOffset.FromUnixTimeSeconds(CreatedAtSeconds + ExpiresInSeconds).UtcDateTime;
}
