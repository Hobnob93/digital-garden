namespace DigitalGarden.Shared.Helpers;

public static class HttpClientHelper
{
    public static void AddDefaultRequestHeaders(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("Content-Security-Policy", "default-src 'self'; frame-ancestors 'self'; form-action 'self'; upgrade-insecure-requests;");
        client.DefaultRequestHeaders.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        client.DefaultRequestHeaders.Add("Permissions-Policy", "geolocation=(), camera=(), microphone=()");
        client.DefaultRequestHeaders.Add("X-Frame-Options", "DENY");
        client.DefaultRequestHeaders.Add("X-Content-Type-Options", "nosniff");
    }
}
