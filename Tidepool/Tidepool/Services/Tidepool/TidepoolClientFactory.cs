using Microsoft.Extensions.Options;

using Pathoschild.Http.Client;

using Tidepool.Model.Tidepool;

namespace Tidepool.Services.Tidepool;

public class TidepoolClientFactory : ITidepoolClientFactory
{
    private readonly IClient _client;
    private readonly TidepoolClientOptions _options;

    public TidepoolClientFactory(IOptions<TidepoolClientOptions> options, HttpClient client)
    {
        _options = options.Value;
        _client = new FluentClient(new Uri(_options.BaseUrl), client);
    }

    private async Task<ITidepoolClient> AuthorizeAsync(string user, string pass)
    {
        var response = await _client
            .PostAsync("auth/login")
            .WithBasicAuthentication(user, pass)
            .AsResponse();

        var token = response.Message.Headers.GetValues("x-tidepool-session-token").Single();
        _client.AddDefault(x => x.WithHeader("x-tidepool-session-token", token));

        var authResponse = await response.As<AuthResponse>();
        var userId = authResponse.Userid;

        return new TidepoolClient(_client, userId);
    }

    public Task<ITidepoolClient> CreateAsync(string user, string pass) =>
        AuthorizeAsync(user, pass);
}