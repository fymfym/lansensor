using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class MonitorAliveMessage
    {
        [JsonProperty("message")] public string Message { get; set; }
    }
}
