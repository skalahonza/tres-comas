using Dexcom.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pathoschild.Http.Client;

namespace Dexcom.Services;

internal class DexcomClientFactory(
    IOptions<DexcomOptions> options,
    HttpClient httpClient,
    ILogger<DexcomClientFactory> logger) : IDexcomClientFactory
{
    private readonly IClient _client = new FluentClient(new Uri(options.Value.BaseUrl), httpClient);
    private DexcomOptions Conf => options.Value;

    public Task<IDexcomClient> Create(string authCode)
        => Authorize(authCode);

    private async Task<IDexcomClient> Authorize(string authCode)
    {
        Dictionary<string, string> formData = new()
        {
            {"client_id", Conf.ClientId },
            {"client_secret", Conf.ClientSecret },
            {"code", authCode },
            {"grant_type", "authorization_code" },
            {"redirect_uri", $"{Conf.CallbackBaseUrl}/dexcom" }
        };
        var content = new FormUrlEncodedContent(formData);
        var response = await httpClient.PostAsync($"{Conf.BaseUrl}/v2/oauth2/token", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var text = await response.Content.ReadAsStringAsync();
            logger.LogError("Error authorizing Dexcom: {Error}", text);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);

        _client.AddDefault(x => x.WithBearerAuthentication(authResponse!.AccessToken!));

        return new DexcomClient(_client);
    }
}
