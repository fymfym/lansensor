using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class KeepAlive
    {
        [JsonProperty("maxMinutesSinceKeepAlive")] public int MaxMinutesSinceKeepAlive { get; set; }
        [JsonProperty("keepAliveDataType")] public string KeepAliveDataType { get; set; }
        [JsonProperty("notifyOnceOnly")] public bool NotifyOnceOnly { get; set; }
    }
}
