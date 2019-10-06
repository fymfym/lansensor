using System.Threading.Tasks;
using LanSensor.Models.Configuration;

namespace LanSensor.PollingMonitor.Services.Monitor.KeepAlive
{
    public interface IKeepAliveMonitor
    {
        Task<bool> IsKeepAliveWithinSpec(DeviceMonitor deviceMonitor);
    }
}
