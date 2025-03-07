using Newtonsoft.Json;

namespace Tidepool.Model.Tidepool;

public class Nutrition
{
    [JsonProperty("carbohydrate")] public Carbohydrate Carbohydrate { get; set; }
}