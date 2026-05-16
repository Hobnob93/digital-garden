using System.Text.Json.Serialization;

namespace DigitalGarden.Shared.Models.Data.Requests;

public record RefreshTraktTokens
(
    [property: JsonPropertyName("refresh_token")] string RefreshToken,
    [property: JsonPropertyName("client_id")] string ClientId,
    [property: JsonPropertyName("client_secret")] string ClientSecret,
    [property: JsonPropertyName("redirect_uri")] string RedirectUri = "urn:ietf:wg:oauth:2.0:oob",
    [property: JsonPropertyName("grant_type")] string GrantType = "refresh_token"
);
