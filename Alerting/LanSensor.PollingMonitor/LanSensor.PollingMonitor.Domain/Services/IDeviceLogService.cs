using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Domain.Services
{
    public interface IDeviceLogService
    {
        Task<DeviceLogEntity> GetLatestPresence(string deviceGroupId, string deviceId);
        Task<DeviceLogEntity> GetLatestKeepAlive(string deviceGroupId, string deviceId);
        Task<DeviceLogEntity> GetLatestPresence(string deviceGroupId, string deviceId, string dataType);
        Task<IEnumerable<DeviceLogEntity>> GetPresenceListSince(string deviceGroupId, string deviceId, DateTime lastKnownPresence);
        Task<IEnumerable<DeviceLogEntity>> GetPresenceListSince(string deviceGroupId, string deviceId, string dataType, DateTime lastKnownPresence);
    }
}
