using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using NLog.Targets.Wrappers;

namespace LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.CalculateAverage
{
    public class CalculateAverageOverHours : IMonitorExecuter
    {
        private readonly IDeviceLogService _deviceLogService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IAlertService _alertService;

        public CalculateAverageOverHours(
            IDeviceLogService deviceLogService,
            IDateTimeService dateTimeService,
            IAlertService alertService
            )
        {
            _deviceLogService = deviceLogService;
            _dateTimeService = dateTimeService;
            _alertService = alertService;
        }

        public bool CanMonitorRun(DeviceMonitor monitor)
        {
            if (monitor.AverageOverHour?.DataValue == null) return false;
            if (monitor.AverageOverHour.AlertBelow == null && monitor.AverageOverHour.AlertAbove == null) return false;
            return monitor.AverageOverHour.Hours >= 1;
        }

        public DeviceStateEntity Run(DeviceStateEntity state, DeviceMonitor monitor)
        {
            var deviceTask = _deviceLogService.GetPresenceListSince(
                monitor.DeviceGroupId,
                monitor.DeviceId,
                monitor.AverageOverHour.DataValue,
                _dateTimeService.Now.AddHours(monitor.AverageOverHour.Hours * -1));

            Task.WaitAll(deviceTask);

            var avg = deviceTask.Result.Average(x => double.Parse(x.DataValue, NumberStyles.Number));

            if (monitor.AverageOverHour.AlertBelow.HasValue)
            {
                if (avg < monitor.AverageOverHour.AlertBelow)
                {
                    _alertService.SendTextMessage(monitor, $"Value below {monitor.AverageOverHour.AlertBelow}");
                }
            }

            if (monitor.AverageOverHour.AlertAbove.HasValue)
            {
                if (avg > monitor.AverageOverHour.AlertAbove)
                {
                    _alertService.SendTextMessage(monitor, $"Value above {monitor.AverageOverHour.AlertAbove}");
                }
            }

            return state;
        }
    }
}
