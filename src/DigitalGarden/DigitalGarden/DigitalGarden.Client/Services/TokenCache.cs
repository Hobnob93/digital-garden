namespace DigitalGarden.Client.Services;

public class TokenCache
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(25);

    public string? AntiforgeryToken { get; private set; }
    public string? SessionToken { get; private set; }

    private DateTime _fetchedAt = DateTime.MinValue;

    public bool NeedsRefresh =>
        AntiforgeryToken is null ||
        SessionToken is null ||
        DateTime.UtcNow - _fetchedAt > TokenLifetime;

    public void Store(string antiforgeryToken, string sessionToken)
    {
        AntiforgeryToken = antiforgeryToken;
        SessionToken = sessionToken;

        _fetchedAt = DateTime.UtcNow;
    }
}
