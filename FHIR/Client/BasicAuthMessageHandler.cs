using System.Net.Http.Headers;
using System.Text;

namespace FHIR.Client;

public class BasicAuthMessageHandler(string username, string password) : DelegatingHandler(new HttpClientHandler())
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
        return base.SendAsync(request, cancellationToken);
    }
}