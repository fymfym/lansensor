using LanSensor.Models.DeviceState;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceState;
using System.Linq;
using System.Threading.Tasks;

namespace LanSensor.PollingMonitor.Services.Monitor.StateChange
{
    public class StateChangeMonitor : IStateChangeMonitor
    {
        private readonly IDeviceStateRepository _deviceStateRepository;
        private readonly IDeviceLogRepository _deviceLogRepository;

        public StateChangeMonitor(
               IDeviceStateRepository deviceStateRepository,
               IDeviceLogRepository deviceLogRepository
            )
        {
            _deviceLogRepository = deviceLogRepository;
            _deviceStateRepository = deviceStateRepository;
        }

        public async Task<StateChangeResult> GetStateChangeNotification(
            string devicegroupId, string deviceId,
            Models.Configuration.StateChangeNotification stateChangeNotification)
        {
            var latestState = await _deviceStateRepository.GetLatestDeviceStateEntity(devicegroupId, deviceId);

            var list = await _deviceLogRepository.GetPresenceListSince(
                devicegroupId, deviceId,
                latestState.LastKnownDataValueDate);

            var presence = list.OrderByDescending(x => x.DateTime).FirstOrDefault();

            StateChangeResult result = null;

            if (presence == null)
                return null;

            if (!string.IsNullOrEmpty(stateChangeNotification.OnDataValueChangeFrom))
            {
                if (string.Equals(stateChangeNotification.OnDataValueChangeFrom, latestState.LastKnownDataValue))
                {
                    result = new StateChangeResult()
                    {
                        DataValue = stateChangeNotification.OnDataValueChangeFrom,
                        ChangedFromValue = true
                    };
                }
            }

            if (!string.IsNullOrEmpty(stateChangeNotification.OnDataValueChangeTo))
            {
                if (string.Equals(stateChangeNotification.OnDataValueChangeTo, presence.DataValue))
                {
                    result = new StateChangeResult()
                    {
                        DataValue = presence.DataValue,
                        ChangedToValue = true
                    };
                }
            }

            latestState.LastKnownDataValue = presence.DataValue;
            latestState.LastKnownDataValueDate = presence.DateTime;
            await _deviceStateRepository.SetDeviceStateEntity(latestState);

            return result;
        }
    }
}
