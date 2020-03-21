using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class DeviceMonitor
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("deviceGroupId")] public string DeviceGroupId { get; set; }
        [JsonProperty("deviceId")] public string DeviceId { get; set; }
        [JsonProperty("messageMediums")] public IEnumerable<MessageMedium> MessageMediums { get; set; }
        [JsonProperty("timeInterval")] public IEnumerable<TimeInterval> TimeIntervals { get; set; }
        [JsonProperty("dataValueToOld")] public DataValueToOld DataValueToOld { get; set; }
        [JsonProperty("keepalive")] public KeepAlive KeepAlive { get; set; }
        [JsonProperty("stateChangeNotification")] public StateChangeNotification StateChangeNotification { get; set; }
        [JsonProperty("averageOverHour")] public AverageOverHour AverageOverHour { get; set; }
    }
}
