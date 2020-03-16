using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ApplicationConfiguration
    {
        [JsonProperty("deviceMonitors")] public IEnumerable<DeviceMonitor> DeviceMonitors { get; set; }
        [JsonProperty("monitor")] public MonitorConfiguration MonitorConfiguration { get; set; }

        // Data stored in environment variables
        //[JsonProperty("mysql")]
        public MySqlConfiguration MySqlConfiguration { get; set; }

        // [JsonProperty("restService")]
        public RestServiceConfiguration RestServiceConfiguration { get; set; }

        //[JsonProperty("mongodb")]
        public MongoConfiguration MongoConfiguration { get; set; }

        //[JsonProperty("slack")]
        public SlackConfiguration SlackConfiguration { get; set; }
    }
}
