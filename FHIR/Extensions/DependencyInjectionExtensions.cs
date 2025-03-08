using FHIR.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FHIR.Extensions;

public static class DependencyInjectionExtensions
{
    public static void AddFhir(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<FhirOptions>(builder.Configuration.GetSection("FHIR"));
        builder.Services.AddSingleton<FhirClientFactory>();
    }
}