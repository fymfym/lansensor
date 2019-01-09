using System.Threading.Tasks;
using LanSensor.Models.DeviceState;

namespace LanSensor.PollingMonitor.Services.Monitor.StateChange
{
    public interface IStateChangeMonitor
    {
        Task<StateChangeResult> GetStateChangeNotification(
            string devicegroupId,
            string deviceId,
            Models.Configuration.StateChangeNotification stateChangeNotification);

    }
}
