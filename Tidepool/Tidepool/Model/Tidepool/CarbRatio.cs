using Newtonsoft.Json;

namespace Tidepool.Model.Tidepool;

public class CarbRatio
{
    [JsonProperty("amount")] public double Amount { get; set; }

    [JsonProperty("start")] public int Start { get; set; }
}