using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanSensor.Repository.DeviceLog
{
    public interface IDeviceLogRepository
    {
        Task<Models.DeviceLog.DeviceLogEntity> GetLatestPresence(string deviceGroupId, string deviceId);
        Task<Models.DeviceLog.DeviceLogEntity> GetLatestKeepalive(string deviceGroupId, string deviceId);
        Task<Models.DeviceLog.DeviceLogEntity> GetLatestPresence(string deviceGroupId, string deviceId, string dataType);
        Task<IEnumerable<Models.DeviceLog.DeviceLogEntity>> GetPresenceListSince(string deviceGroupId, string deviceId, DateTime lastKnownPresence);
    }
}
