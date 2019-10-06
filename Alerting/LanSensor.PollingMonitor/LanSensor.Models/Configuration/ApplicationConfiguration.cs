using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ApplicationConfiguration
    {
        [JsonProperty("deviceMonitors")] public IEnumerable<DeviceMonitor> DeviceMonitors { get; set; }
        [JsonProperty("mysql")] public MySqlConfiguration MySqlConfiguration { get; set; }
        [JsonProperty("litedb")] public LiteDbConfiguration LiteDbConfiguration { get; set; }
        [JsonProperty("slack")] public SlackConfiguration SlackConfiguration { get; set; }
        [JsonProperty("monitor")] public MonitorConfiguration MonitorConfiguration { get; set; }
    }
}
