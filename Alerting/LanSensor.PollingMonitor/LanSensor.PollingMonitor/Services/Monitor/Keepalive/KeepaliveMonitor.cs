using System;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.Repository.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor.Keepalive
{
    public class KeepaliveMonitor : IKeepaliveMonitor
    {
        private readonly IDeviceLogRepository _repository;
        private readonly IGetDateTime _dateTime;

        public KeepaliveMonitor
            (
                IDeviceLogRepository repository,
                IGetDateTime dateTime
            )
        {
            _repository = repository;
            _dateTime = dateTime;
        }

        public async Task<bool> IsKeepAliveWithinSpec(DeviceMonitor monitor)
        {
            if (monitor?.DeviceGroupId == null || monitor?.DeviceId == null || monitor?.KeepAlive == null)
            {
                return true;
            }

            var keepAlive = await _repository.GetLatestPresence(monitor.DeviceGroupId, monitor.DeviceId,
                                monitor.KeepAlive.KeepAliveDataType);
            if (keepAlive == null)
                return false;

            var ts = new TimeSpan(_dateTime.Now.Ticks - keepAlive.DateTime.Ticks);
            return (ts.TotalMinutes <=
                   monitor.KeepAlive.MaxMinutesSinceKeepAlive);
        }
    }
}
