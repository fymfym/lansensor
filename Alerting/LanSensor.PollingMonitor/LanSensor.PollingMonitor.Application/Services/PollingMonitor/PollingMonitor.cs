using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using NLog;

namespace LanSensor.PollingMonitor.Application.Services.PollingMonitor
{
    public class PollingMonitor : IPollingMonitor
    {
        private readonly IEnumerable<IMonitorExecuter> _monitorExecuterList;
        private readonly IServiceConfiguration _configuration;
        private readonly IAlertService _alert;
        private readonly IDeviceStateService _deviceStateRepositoryService;
        private readonly ILogger _logger;
        private readonly IPauseService _pauseService;
        private readonly IMonitorTools _monitorTools;
        private readonly IDateTimeService _dateTimeService;

        public PollingMonitor
        (
            IServiceConfiguration configuration,
            IAlertService alert,
            IDeviceStateService deviceStateRepositoryService,
            IEnumerable<IMonitorExecuter> monitorExecuterList,
            ILogger logger,
            IPauseService pauseService,
            IMonitorTools monitorTools,
            IDateTimeService dateTimeService
        )
        {
            _logger = logger;
            _pauseService = pauseService;
            _monitorTools = monitorTools;
            _dateTimeService = dateTimeService;
            _deviceStateRepositoryService = deviceStateRepositoryService;
            _configuration = configuration ?? throw new Exception("Add configuration");
            _alert = alert;
            _monitorExecuterList = monitorExecuterList;
            StoppedIntentionally = false;
        }

        public bool StoppedIntentionally { get; private set; }

        public void Stop()
        {
            _logger.Info("Stopping intentionally");
            StoppedIntentionally = true;
        }

        public void RunThroughDeviceMonitors()
        {
            _logger.Debug("RunThroughDeviceMonitors");

            foreach (var deviceMonitor in _configuration.ApplicationConfiguration.DeviceMonitors)
            {
                if (deviceMonitor == null) continue;

                if (!_monitorTools.IsInsideTimeInterval(deviceMonitor.TimeIntervals, _dateTimeService.Now))
                {
                    continue;
                }

                var deviceStateTask = _deviceStateRepositoryService.GetDeviceState(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);
                Task.WaitAll(deviceStateTask);
                var deviceState = _monitorExecuterList
                    .Where(executer => executer
                        .CanMonitorRun(deviceMonitor))
                    .Aggregate(deviceStateTask.Result, (current, executer) => executer
                        .Run(current, deviceMonitor));

                _deviceStateRepositoryService.SaveDeviceState(deviceState).Wait();
            }

            var count = _configuration.ApplicationConfiguration.MonitorConfiguration.PollingIntervalSeconds;
            while (count > 0)
            {
                _pauseService.Pause(1000);
                count--;
                if (StoppedIntentionally) break;
            }
        }

        public int RunInLoop()
        {
            _logger.Info("RunInLoop starting");

            var systemDeviceMonitor = _configuration.ApplicationConfiguration.DeviceMonitors.FirstOrDefault(x =>
                x.DeviceGroupId.Equals("system", StringComparison.CurrentCultureIgnoreCase));

            if (systemDeviceMonitor != null)
                _alert.SendTextMessage(systemDeviceMonitor, "Monitor starting");

            _logger.Info("Starting run loop");

            var lastKeepAlive = DateTime.Now;


            while (!StoppedIntentionally)
            {
                RunThroughDeviceMonitors();
                if (new TimeSpan(lastKeepAlive.Ticks - DateTime.Now.Ticks).Hours > 24 && DateTime.Now.Hour == 8)
                {
                    _alert.SendTextMessage(null, "My daily 8 o'clock check-in");
                    lastKeepAlive = DateTime.Now;
                }
            }

            return 1;
        }
    }
}
