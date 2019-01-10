using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class MonitorConfiguration
    {
        [JsonIgnore]
        [JsonProperty("pollingIntervalSeconds")] public int PollingIntervalSeconds { get; set; }

    }
}
