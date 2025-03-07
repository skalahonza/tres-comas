namespace FHIR;

public class ApiKeyMessageHandler : DelegatingHandler
{
    private readonly string _apiKey;

    public ApiKeyMessageHandler(string apiKey) : base(new HttpClientHandler()) => 
        _apiKey = apiKey;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("x-api-key", _apiKey);
        return base.SendAsync(request, cancellationToken);
    }
}