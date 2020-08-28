using System.Collections.Generic;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public interface IApplicationConfiguration
    {
        IEnumerable<DeviceMonitor> DeviceMonitors { get; set; }
        MonitorConfiguration MonitorConfiguration { get; set; }

        MySqlConfiguration MySqlConfiguration { get; set; }

        RestServiceConfiguration RestServiceConfiguration { get; set; }

        MongoDbConfiguration MongoDbConfiguration { get; set; }

        SlackConfiguration SlackConfiguration { get; set; }
    }
}
