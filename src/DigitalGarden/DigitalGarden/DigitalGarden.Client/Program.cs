using DigitalGarden.Client.MessageHandlers;
using DigitalGarden.Client.Services;
using DigitalGarden.Client.Services.Implementations;
using DigitalGarden.Shared.Constants;
using DigitalGarden.Shared.Helpers;
using DigitalGarden.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;

var baseEnvAddress = builder.HostEnvironment.BaseAddress;
services.AddHttpClient(ApiConstants.MainClientName,
    (sp, client) =>
    {
        client.BaseAddress = new Uri($"{baseEnvAddress}api/");
        HttpClientHelper.AddDefaultRequestHeaders(client);
    }).AddHttpMessageHandler<TokenHandler>();

services.AddHttpClient(ApiConstants.TokenClientName,
    (sp, client) =>
    {
        client.BaseAddress = new Uri($"{baseEnvAddress}");
        HttpClientHelper.AddDefaultRequestHeaders(client);
    });

services.AddSingleton<TokenCache>();
services.AddTransient<TokenHandler>();

services.AddTransient<ISiteConfigurationProvider, SiteConfigurationClient>();
services.AddTransient<IBeaconProvider, BeaconClient>();
services.AddTransient<ILifeDataProvider, LifeDataClient>();

await builder.Build().RunAsync();
