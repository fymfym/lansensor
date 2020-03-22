using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MonitorConfiguration
    {
        [JsonProperty("pollingIntervalSeconds")] public int PollingIntervalSeconds { get; set; }
    }
}
