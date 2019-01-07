using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class DeviceMonitors
    {
        [JsonProperty("deviceMonitors")] public IEnumerable<DeviceMonitor> DeviceMonitores { get; set; }
        [JsonProperty("mySql")] public MySqlConfiguration MySqlConfiguration { get; set; }
    }
}
