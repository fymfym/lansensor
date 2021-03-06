﻿using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MessageMedium
    {
        [JsonProperty("mediumType")] public string MediumType { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
    }
}
