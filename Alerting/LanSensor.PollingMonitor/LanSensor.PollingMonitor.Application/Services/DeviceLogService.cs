using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.Repository.Repositories;

namespace LanSensor.PollingMonitor.Application.Services
{
    public class DeviceLogService : IDeviceLogService
    {
        private readonly IDeviceLogRepository _deviceLogRepository;

        public DeviceLogService(
            IDeviceLogRepository deviceLogRepository
            )
        {
            _deviceLogRepository = deviceLogRepository;
        }

        public Task<DeviceLogEntity> GetLatestPresence(string deviceGroupId, string deviceId)
        {
            return _deviceLogRepository.GetLatestPresence(deviceGroupId, deviceId);
        }

        public Task<DeviceLogEntity> GetLatestKeepAlive(string deviceGroupId, string deviceId)
        {
            return _deviceLogRepository.GetLatestKeepAlive(deviceGroupId, deviceId);
        }

        public Task<DeviceLogEntity> GetLatestPresence(string deviceGroupId, string deviceId, string dataType)
        {
            return _deviceLogRepository.GetLatestPresence(deviceGroupId, deviceId, dataType);
        }

        public Task<IEnumerable<DeviceLogEntity>> GetPresenceListSince(string deviceGroupId, string deviceId, DateTime lastKnownPresence)
        {
            return _deviceLogRepository.GetPresenceListSince(deviceGroupId, deviceId, lastKnownPresence);
        }

        public Task<IEnumerable<DeviceLogEntity>> GetPresenceListSince(string deviceGroupId, string deviceId, string dataType, DateTime lastKnownPresence)
        {
            return _deviceLogRepository.GetPresenceListSince(deviceGroupId, deviceId, dataType, lastKnownPresence);
        }
    }
}
