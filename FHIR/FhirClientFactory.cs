using Hl7.Fhir.Rest;
using Microsoft.Extensions.Options;

namespace FHIR;

public class FhirClientFactory
{
    public FhirClientFactory(IOptions<FhirOptions> options)
    {
        var url = options.Value.Url;
        var username = options.Value.Username;
        var password = options.Value.Password;
        var client = new FhirClient(url, messageHandler: new BasicAuthMessageHandler(username, password));
        client.Settings.PreferredFormat = ResourceFormat.Json;
        client.Settings.PreferredReturn = Prefer.ReturnRepresentation;
        Client = client;
    }

    public FhirClient Client { get; }
}