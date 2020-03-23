using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.CalculateAverage
{
    public class CalculateAverageOverHoursMonitor : IMonitorExecuter
    {
        private readonly IDeviceLogService _deviceLogService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IAlertService _alertService;
        private readonly IMonitorTools _monitorTools;

        public CalculateAverageOverHoursMonitor(
            IDeviceLogService deviceLogService,
            IDateTimeService dateTimeService,
            IAlertService alertService,
            IMonitorTools monitorTools
            )
        {
            _deviceLogService = deviceLogService;
            _dateTimeService = dateTimeService;
            _alertService = alertService;
            _monitorTools = monitorTools;
        }

        public bool CanMonitorRun(DeviceMonitor monitor)
        {
            if (monitor?.DeviceGroupId == null || monitor.DeviceId == null) return false;
            if (monitor?.AverageOverHour?.DataValue == null) return false;
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

            var monitorState = _monitorTools.GetMonitorState(state, monitor);
            if (monitorState?.Value != null)
            {
                DateTime.TryParse(monitorState.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var lastRun);
                if (lastRun > DateTime.MinValue)
                {
                    var ts = new TimeSpan(_dateTimeService.Now.Ticks - lastRun.Ticks);
                    if (ts.TotalMinutes < 5) return state;
                }
            }

            var avg = deviceTask.Result.Average(x => double.Parse(x.DataValue, NumberStyles.Number));

            if (!monitor.AverageOverHour.AlertBelow.HasValue && !monitor.AverageOverHour.AlertAbove.HasValue)
            {
                _alertService.SendTextMessage(monitor, $"Average is {avg}");
                _monitorTools.SetMonitorState(state, monitor, new MonitorState
                {
                    Value = _dateTimeService.Now.ToString(CultureInfo.InvariantCulture)
                });
            }

            if (monitor.AverageOverHour.AlertBelow.HasValue)
            {
                if (avg < monitor.AverageOverHour.AlertBelow)
                {
                    _alertService.SendTextMessage(monitor, $"Value below {monitor.AverageOverHour.AlertBelow}");
                    _monitorTools.SetMonitorState(state, monitor, new MonitorState
                    {
                        Value = _dateTimeService.Now.ToString(CultureInfo.InvariantCulture)
                    });
                }
            }

            if (!monitor.AverageOverHour.AlertAbove.HasValue) return state;

            if (avg <= monitor.AverageOverHour.AlertAbove) return state;

            _alertService.SendTextMessage(monitor, $"Value above {monitor.AverageOverHour.AlertAbove}");
            _monitorTools.SetMonitorState(state, monitor, new MonitorState
            {
                Value = _dateTimeService.Now.ToString(CultureInfo.InvariantCulture)
            });

            return state;
        }
    }
}
