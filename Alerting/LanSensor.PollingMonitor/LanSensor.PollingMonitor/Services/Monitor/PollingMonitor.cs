using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Pause;
using NLog;

namespace LanSensor.PollingMonitor.Services.Monitor
{
    public class PollingMonitor : IPollingMonitor
    {
        private readonly IEnumerable<IMonitorExecuter> _monitorExecuterList;
        private readonly IServiceConfiguration _configuration;
        private readonly IAlert _alert;
        private readonly IDeviceStateService _deviceStateRepositoryService;
        private readonly ILogger _logger;
        private readonly IPauseService _pauseService;

        public PollingMonitor
        (
            IServiceConfiguration configuration,
            IAlert alert,
            IDeviceStateService deviceStateRepositoryService,
            IEnumerable<IMonitorExecuter> monitorExecuterList,
            ILogger logger,
            IPauseService pauseService
        )
        {
            _logger = logger;
            _pauseService = pauseService;
            _deviceStateRepositoryService = deviceStateRepositoryService;
            _configuration = configuration ?? throw new Exception("Add configurtation");
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
            foreach (var deviceMonitor in _configuration.ApplicationConfiguration.DeviceMonitors)
            {
                if (deviceMonitor == null) continue;

                var deviceStateTask = _deviceStateRepositoryService.GetDeviceState(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);
                Task.WaitAll(deviceStateTask);
                var deviceState = deviceStateTask.Result;

                foreach (var executer in _monitorExecuterList)
                {
                    if (executer.CanMonitorRun(deviceMonitor))
                    {
                        var task = executer.Run(deviceState, deviceMonitor);
                    }
                }

                _deviceStateRepositoryService.SaveDeviceState(deviceState).Wait();

                //_logger.Info($"Device monitor - {deviceMonitor.Name} DeviceGroupId:<{deviceMonitor.DeviceGroupId}>, DeviceId:<{deviceMonitor.DeviceId}>, DataType:<{deviceMonitor.DataType}>");

                //if (deviceMonitor.KeepAlive != null)
                //{
                //    deviceState = KeepAlive(deviceState, deviceMonitor);
                //}

                //var presenceRecordTask = _deviceLogService.GetLatestPresence(
                //    deviceMonitor.DeviceGroupId,
                //    deviceMonitor.DeviceId,
                //    deviceMonitor.DataType);

                //var deviceLogTask = _deviceLogService.GetLatestPresence(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId, deviceMonitor.DataType);

                //var keepAliveTask = _keepAliveMonitor.IsKeepAliveWithinSpec(deviceMonitor);

                //Task.WaitAll(presenceRecordTask, latestKeepAliveTask, deviceLogTask, latestStateTask);

                //if (deviceMonitor.DataType == "FrontDoorOpen")
                //{
                //    _logger.Debug("FrontDoor");
                //}


                //var deviceLog = deviceLogTask.Result ?? new DeviceLogEntity();


                //var keepAlive = keepAliveTask.Result;
                //var presenceRecord = presenceRecordTask.Result;


                //if (deviceMonitor.TimeIntervals != null)
                //{
                //    var failedTimeInterval = _stateCheckMonitor.GetFailedTimerInterval(deviceMonitor.TimeIntervals, presenceRecord);
                //    if (failedTimeInterval != null)
                //    {
                //        _logger.Info(
                //            $"SendTimerIntervalAlert {presenceRecord}, {failedTimeInterval}, {deviceMonitor}");
                //        _alert.SendTimerIntervalAlert(presenceRecord, failedTimeInterval, deviceMonitor);
                //    }
                //}

                //if (deviceMonitor.DataValueToOld != null)
                //{
                //    _dataValueToOldMonitor.IsDataValueToOld()
                //}

                //if (deviceMonitor.StateChangeNotification != null)
                //{
                //    if (deviceMonitor.StateChangeNotification.OnEveryChange)
                //    {
                //        _logger.Info("State change");
                //        var stateChange = _stateChange.GetStateChangeNotification(
                //            latestState, deviceLog,
                //            deviceMonitor.StateChangeNotification);
                //        if (stateChange != null)
                //        {
                //            _alert.SendStateChangeAlert(stateChange, deviceMonitor);
                //        }
                //    }

                //    var stateOnChange = _stateChange.GetStateChangeFromToNotification(
                //        latestState, deviceLog,
                //        deviceMonitor.StateChangeNotification);
                //    if (stateOnChange != null)
                //    {
                //        _logger.Info("_alert.SendStateChangeAlert");
                //        _alert.SendStateChangeAlert(stateOnChange, deviceMonitor);
                //    }
                //}

                //latestState.DeviceGroupId = deviceMonitor.DeviceGroupId;
                //latestState.DeviceId = deviceMonitor.DeviceId;
                //latestState.LastKnownDataValue = deviceLog.DataValue;
                //latestState.LastExecutedKeepAliveCheckDate = System.DateTime.Now;
                //latestState.LastKnownDataValueDate = deviceLog.DateTime;
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

            while (!StoppedIntentionally)
            {
                RunThroughDeviceMonitors();
            }

            return 1;
        }
    }
}
