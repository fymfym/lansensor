using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class RestServiceConfiguration
    {
        [JsonProperty("deviceRestApiBasePath")] public string DeviceRestApiBasePath { get; set; }
    }
}
