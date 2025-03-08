using Newtonsoft.Json;

namespace Dexcom.Models;

internal class AuthResponse
{
    [JsonProperty("access_token")]
    public required string AccessToken { get; set; }
}
