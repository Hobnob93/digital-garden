using DigitalGarden.Client.MessageHandlers;
using DigitalGarden.Client.Services.Implementations;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;

services.AddHttpClient(ApiConstants.MainClientName,
    (sp, client) =>
    {
        client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
        AddDefaultRequestHeaders(client);
    }).AddHttpMessageHandler<TokenHandler>();

services.AddHttpClient(ApiConstants.TokenClient,
    (sp, client) =>
    {
        client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}");
        AddDefaultRequestHeaders(client);
    });

services.AddSingleton<TokenHandler>();
services.AddTransient<ISiteConfigurationProvider, SiteConfigurationClient>();
services.AddTransient<IBeaconProvider, BeaconClient>();
services.AddTransient<ILifeDataProvider, LifeDataClient>();

await builder.Build().RunAsync();

static void AddDefaultRequestHeaders(HttpClient client)
{
    client.DefaultRequestHeaders.Add("Content-Security-Policy", "default-src 'self'; frame-ancestors 'self'; form-action 'self'; upgrade-insecure-requests;");
    client.DefaultRequestHeaders.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    client.DefaultRequestHeaders.Add("Permissions-Policy", "geolocation=(), camera=(), microphone=()");
    client.DefaultRequestHeaders.Add("X-Frame-Options", "DENY");
    client.DefaultRequestHeaders.Add("X-Content-Type-Options", "nosniff");
}
