using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MonitorConfiguration
    {
        [JsonProperty("pollingIntervalSeconds")] public int PollingIntervalSeconds { get; set; }
    }
}
