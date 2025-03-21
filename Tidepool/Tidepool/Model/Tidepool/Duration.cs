﻿using Newtonsoft.Json;

namespace Tidepool.Model.Tidepool;

public class Duration
{
    [JsonProperty("units")] public string Units { get; set; }

    [JsonProperty("value")] public double Value { get; set; }
}