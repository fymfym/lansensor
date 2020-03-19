using System;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.DataValueToOld
{
    public class DataValueToOldMonitor : IMonitorExecuter
    {
        private readonly IDeviceLogService _deviceLogService;
        private readonly IAlertService _alertService;
        private readonly IDateTimeService _dateTimeService;

        public DataValueToOldMonitor(
            IDeviceLogService deviceLogService,
            IAlertService alertService,
            IDateTimeService dateTimeService
            )
        {
            _deviceLogService = deviceLogService;
            _alertService = alertService;
            _dateTimeService = dateTimeService;
        }

        public bool CanMonitorRun(DeviceMonitor monitor)
        {
            if (monitor?.DeviceGroupId == null || monitor.DeviceId == null) return false;
            return monitor.DataValueToOld?.MaxAgeInMinutes > 0 && monitor.DataType != null;
        }

        public DeviceStateEntity Run(DeviceStateEntity state, DeviceMonitor monitor)
        {
            if (!CanMonitorRun(monitor)) return state;

            var deviceLogEntityTask = _deviceLogService.GetLatestPresence(
                    monitor.DeviceGroupId,
                    monitor.DeviceId, 
                    monitor.DataType);

            Task.WaitAll(deviceLogEntityTask);

            var date = _dateTimeService.Now;
            var span = new TimeSpan(date.Ticks - deviceLogEntityTask.Result.DateTime.Ticks);
            if (span.TotalMinutes < monitor.DataValueToOld.MaxAgeInMinutes)
            {
                _alertService.SendTimerIntervalAlert(deviceLogEntityTask.Result, monitor);
            }

            return state;
        }
    }
}
