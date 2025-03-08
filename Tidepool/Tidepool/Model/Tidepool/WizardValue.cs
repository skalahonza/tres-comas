using Newtonsoft.Json;

namespace Tidepool.Model.Tidepool;

public class WizardValue
{
    [JsonProperty("id")]
    public required string Id { get; set; }
    [JsonProperty("carbInput")]
    public required decimal CarbInput { get; set; }
    [JsonProperty("time")]
    public required DateTime Time { get; set; }
}
