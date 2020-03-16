using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Domain.Repositories
{
    public interface IServiceConfiguration
    {
        ApplicationConfiguration ApplicationConfiguration { get; set; }
    }
}
