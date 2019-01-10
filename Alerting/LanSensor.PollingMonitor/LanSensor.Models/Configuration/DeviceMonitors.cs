using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class DeviceMonitors
    {
        [JsonIgnore, JsonProperty("deviceMonitors")] public IEnumerable<DeviceMonitor> DeviceMonitores { get; set; }
        [JsonIgnore, JsonProperty("mySql")] public MySqlConfiguration MySqlConfiguration { get; set; }
    }
}
