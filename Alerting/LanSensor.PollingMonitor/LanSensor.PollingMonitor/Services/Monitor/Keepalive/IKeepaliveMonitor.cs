using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Services.Monitor.KeepAlive
{
    public interface IKeepAliveMonitor
    {
        Task<bool> IsKeepAliveWithinSpec(DeviceMonitor deviceMonitor);
    }
}
