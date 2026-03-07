namespace DigitalGarden.Shared.Constants;

public static class ApiConstants
{
    public const string MainClientName = "MainClient";
    public const string LastFmClientName = "LastFmClient";
    public const string TokenClient = "AntiforgeryClient";

    public const string BlazorClientCorsPolicyName = "BlazorClientOnly";
    public const string SessionTokenName = "ApiToken";

    public const string AntiforgeryTokenHeader = "X-XSRF-TOKEN";
    public const string SessionTokenHeader = "X-Session-Token";
}
