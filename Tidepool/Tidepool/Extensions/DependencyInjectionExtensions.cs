using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Tidepool.Services.Tidepool;

namespace Tidepool.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddTidepoolClient(this IServiceCollection services,
        Action<TidepoolClientOptions, IConfiguration> configureOptions)
    {
        services.AddHttpClient<ITidepoolClientFactory, TidepoolClientFactory>();
        services.AddOptions<TidepoolClientOptions>().Configure(configureOptions);
        return services;
    }
}