using System;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.DateTime;

namespace LanSensor.PollingMonitor.Services.Monitor.KeepAlive
{
    public class KeepAliveMonitor : IMonitorExecuter
    {
        private readonly IDeviceLogService _deviceLogService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IAlert _alert;

        public KeepAliveMonitor
            (
                IDeviceLogService deviceLogService,
                IDateTimeService dateTimeService,
                IAlert alert
            )
        {
            _deviceLogService = deviceLogService;
            _dateTimeService = dateTimeService;
            _alert = alert;
        }

        public bool CanMonitorRun(DeviceMonitor monitor)
        {
            return monitor?.DeviceId != null
                   && monitor.DeviceGroupId != null
                   && monitor.KeepAlive?.KeepAliveDataType != null
                   && monitor.KeepAlive.MaxMinutesSinceKeepAlive > 0;
        }

        public DeviceStateEntity Run(DeviceStateEntity state, DeviceMonitor monitor)
        {
            if (!CanMonitorRun(monitor)) throw new Exception("Can not run om this monitor");

            var keepAliveTask = _deviceLogService.GetLatestPresence(
                monitor.DeviceGroupId, monitor.DeviceId,
                monitor.KeepAlive.KeepAliveDataType);

            Task.WaitAll(keepAliveTask);

            var keepAlive = keepAliveTask.Result;

            var ts = new TimeSpan(_dateTimeService.Now.Ticks - keepAlive.DateTime.Ticks);

            var sendKeepAlive = ts.TotalMinutes > monitor.KeepAlive.MaxMinutesSinceKeepAlive;

            if (monitor.KeepAlive.NotifyOnceOnly)
                sendKeepAlive = state.LastKeepAliveAlert < state.LastKnownKeepAliveDate;

            if (sendKeepAlive)
                _alert.SendKeepAliveMissingAlert(monitor);

            state.LastKnownKeepAliveDate = keepAlive.DateTime;

            return state;
        }
    }
}
