using System;
using System.Globalization;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.MonitorAliveMessage
{
    public class MonitorAliveMessageMonitor : IMonitorExecuter
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly IAlertService _alert;
        private readonly IMonitorTools _monitorTools;

        public MonitorAliveMessageMonitor
            (
                IDateTimeService dateTimeService,
                IAlertService alert,
                IMonitorTools monitorTools
            )
        {
            _dateTimeService = dateTimeService;
            _alert = alert;
            _monitorTools = monitorTools;
        }

        public bool CanMonitorRun(DeviceMonitor monitor)
        {
            return monitor?.MonitorAliveMessage?.Message != null;
        }

        public DeviceStateEntity Run(DeviceStateEntity state, DeviceMonitor monitor)
        {
            if (!CanMonitorRun(monitor)) throw new Exception("Can not run on this monitor");

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

            _alert.SendTextMessage(monitor, monitor.MonitorAliveMessage.Message);
            _monitorTools.SetMonitorState(state, monitor, new MonitorState
            {
                Value = _dateTimeService.Now.ToString(CultureInfo.InvariantCulture)
            });

            return state;
        }
    }
}
