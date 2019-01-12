using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DeviceMonitor
    {
        [JsonProperty("deviceGroupId")] public string DeviceGroupId { get; set; }
        [JsonProperty("deviceId")] public string DeviceId { get; set; }
        [JsonProperty("dataType")] public string DataType { get; set; }
        [JsonProperty("messageMediums")] public IEnumerable<MessageMedium> MessageMediums { get; set; }
        [JsonProperty("timeInterval")] public IEnumerable<TimeInterval> TimeIntervals { get; set; }
        [JsonProperty("keepalive")] public Keepalive Keepalive { get; set; }
        [JsonProperty("stateChangeNotification")] public StateChangeNotification StateChangeNotification { get; set; }
    }
}
