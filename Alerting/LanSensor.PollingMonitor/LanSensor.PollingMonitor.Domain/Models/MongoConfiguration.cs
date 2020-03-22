﻿using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MongoConfiguration
    {
        [JsonProperty("connectionString")] public string ConnectionString { get; set; }
    }
}
