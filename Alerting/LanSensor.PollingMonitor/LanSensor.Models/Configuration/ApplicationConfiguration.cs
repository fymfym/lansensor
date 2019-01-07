using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class ApplicationConfiguration
    {
        [JsonProperty("deviceMonitors")] public IEnumerable<DeviceMonitor> DeviceMonitors { get; set; }
        [JsonProperty("mysql")] public MySqlConfiguration MySqlConfiguration{ get; set; }
        [JsonProperty("slack")] public SlackConfiguration SlackConfiguration{ get; set; }

    }
}
