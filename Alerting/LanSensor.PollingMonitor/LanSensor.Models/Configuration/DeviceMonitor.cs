using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public  class DeviceMonitor
    {
        [JsonIgnore, JsonProperty("deviceGroupId")] public string DeviceGroupId { get; set; }
        [JsonIgnore, JsonProperty("deviceId")] public string DeviceId { get; set; }
        [JsonIgnore, JsonProperty("dataType")] public string DataType { get; set; }
        [JsonIgnore, JsonProperty("state")] public string State { get; set; }
        [JsonIgnore, JsonProperty("receiver")] public IEnumerable<MessageMedium> MessageMediums { get; set; }
        [JsonIgnore, JsonProperty("timeInterval")] public IEnumerable<TimeInterval> TimeIntervals { get; set; }      
        [JsonIgnore, JsonProperty("keepalive")] public Keepalive Keepalive { get; set; }      
        [JsonIgnore, JsonProperty("stateChangeNotification")] public StateChangeNotification StateChangeNotification { get; set; }      
    }
}
