using Dexcom.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dexcom;

public static class DependencyInjection
{
    public static IServiceCollection AddDexcom(this IServiceCollection services, Action<DexcomOptions, IConfiguration> configureOptions)
    {
        services.AddOptions<DexcomOptions>().Configure(configureOptions);
        services.AddHttpClient<IDexcomClientFactory, DexcomClientFactory>();

        return services;
    }
}
