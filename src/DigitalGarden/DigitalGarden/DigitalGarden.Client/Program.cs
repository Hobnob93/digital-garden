using DigitalGarden.Client.Services.Implementations;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;

services.AddHttpClient(ApiClientNames.AnonymousClientName,
    (sp, client) =>
    {
        client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/");
        AddDefaultRequestHeaders(client);
    });

services.AddTransient<ISiteConfigurationProvider, SiteConfigurationClient>();
services.AddTransient<IBeaconProvider, BeaconClient>();

await builder.Build().RunAsync();

void AddDefaultRequestHeaders(HttpClient client)
{
    client.DefaultRequestHeaders.Add("Content-Security-Policy", "default-src 'self'; frame-ancestors 'self'; form-action 'self'; upgrade-insecure-requests;");
    client.DefaultRequestHeaders.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    client.DefaultRequestHeaders.Add("Permissions-Policy", "geolocation=(), camera=(), microphone=()");
    client.DefaultRequestHeaders.Add("X-Frame-Options", "DENY");
    client.DefaultRequestHeaders.Add("X-Content-Type-Options", "nosniff");
}
