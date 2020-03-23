using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ApplicationConfiguration
    {
        [JsonProperty("deviceMonitors")] public IEnumerable<DeviceMonitor> DeviceMonitors { get; set; }
        [JsonProperty("monitor")] public MonitorConfiguration MonitorConfiguration { get; set; }

        //Data stored in environment variables
        public MySqlConfiguration MySqlConfiguration { get; set; }

        public RestServiceConfiguration RestServiceConfiguration { get; set; }

        public MongoDbConfiguration MongoDbConfiguration { get; set; }

        public SlackConfiguration SlackConfiguration { get; set; }
    }
}
