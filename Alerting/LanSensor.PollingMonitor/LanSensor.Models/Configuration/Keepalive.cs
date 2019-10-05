using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class KeepAlive
    {
        [JsonProperty("maxMinutesSinceKeepalive")] public int MaxMinutesSinceKeepAlive { get; set; }
        [JsonProperty("keepaliveDataType")] public string KeepAliveDataType { get; set; }
        [JsonProperty("notifyOnceOnly")] public bool NotifyOnceOnly { get; set; }
    }
}
