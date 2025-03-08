using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dexcom;

public static class DependencyInjection
{
    public static IServiceCollection AddDexcom(this IServiceCollection services, IConfiguration configuration)
    {

        return services;
    }
}
