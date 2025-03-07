using Newtonsoft.Json;

namespace Tidepool.Model.Tidepool;

public class BasalSchedule
{
    [JsonProperty("rate")] public double Rate { get; set; }

    [JsonProperty("start")] public int Start { get; set; }
}