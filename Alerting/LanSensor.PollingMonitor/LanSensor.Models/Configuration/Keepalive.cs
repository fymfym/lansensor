using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class Keepalive
    {
        [JsonProperty("maxMinutesSinceKeepalive")] public int MaxMinutesSinceKeepalive { get; set; }
        [JsonProperty("keepaliveDataType")] public string KeepaliveDataType { get; set; }
        [JsonProperty("notifyOnceOnly")] public bool NotifyOnceOnly { get; set; }
    }
}
