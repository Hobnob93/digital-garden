using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace Website.Store.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddStateManagement(this IServiceCollection services)
    {
        services.AddFluxor(options =>
        {
            options.ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly);
#if DEBUG
            options.UseReduxDevTools();
#endif
        });
    }
}
