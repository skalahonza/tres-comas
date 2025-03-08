namespace Dexcom.Services;

public class DexcomOptions
{
    public required string BaseUrl { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string CallbackBaseUrl { get; set; }
}
