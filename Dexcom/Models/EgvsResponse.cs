using Newtonsoft.Json;

namespace Dexcom.Models;

public class EgvsResponse
{
    [JsonProperty("records")]
    public ICollection<EgvRecord> Records { get; set; } = [];
}

public class EgvRecord
{
    [JsonProperty("recordId")]
    public required string RecordId { get; set; }
    [JsonProperty("systemTime")]
    public required DateTime SystemTime { get; set; }
    [JsonProperty("value")]
    public required double Value { get; set; }
    [JsonProperty("unit")]
    public required string Unit { get; set; }
}
