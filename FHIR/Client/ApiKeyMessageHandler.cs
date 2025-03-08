namespace FHIR.Client;

public class ApiKeyMessageHandler(string apiKey) : DelegatingHandler(new HttpClientHandler())
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("x-api-key", apiKey);
        return base.SendAsync(request, cancellationToken);
    }
}