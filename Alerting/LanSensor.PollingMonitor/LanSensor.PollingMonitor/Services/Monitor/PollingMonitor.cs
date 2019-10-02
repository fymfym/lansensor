using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Monitor.Keepalive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceState;
using NLog;

namespace LanSensor.PollingMonitor.Services.Monitor
{
    public class PollingMonitor : IPollingMonitor
    {
        private readonly IConfiguration _configuration;
        private readonly IDeviceLogRepository _dataStore;
        private readonly IAlert _alert;
        private readonly ITimeIntervalMonitor _stateCheckMonitor;
        private readonly IKeepaliveMonitor _keepAliveMonitor;
        private readonly IStateChangeMonitor _stateChange;
        private readonly IDeviceStateRepository _deviceStateRepository;
        private readonly IDeviceLogRepository _deviceLogRepository;
        private readonly ILogger _logger;

        private bool _stop;

        public PollingMonitor
        (
            IConfiguration configuration,
            IDeviceLogRepository dataStore,
            IAlert alert,
            ITimeIntervalMonitor stateCheckMonitor,
            IKeepaliveMonitor keepAliveMonitor,
            IStateChangeMonitor stateChange,
            IDeviceStateRepository deviceStateRepository,
            IDeviceLogRepository deviceLogRepository,
            ILogger logger
        )
        {
            _deviceLogRepository = deviceLogRepository;
            _logger = logger;
            _deviceStateRepository = deviceStateRepository;
            _configuration = configuration ?? throw new Exception("Add configurtation");
            _dataStore = dataStore;
            _alert = alert;
            _stateCheckMonitor = stateCheckMonitor;
            _keepAliveMonitor = keepAliveMonitor;
            _stateChange = stateChange;
            _stop = false;
        }

        public bool StoppedIntentionally => _stop;

        public void Stop()
        {
            _logger.Info("Stopping intentionally");
            _stop = true;
        }

        public int Run()
        {
            _logger.Info("Run starting");

            var systemDeviceMonitor = _configuration.ApplicationConfiguration.DeviceMonitors.FirstOrDefault(x =>
                x.DeviceGroupId.Equals("system", StringComparison.CurrentCultureIgnoreCase));

            if (systemDeviceMonitor != null)
                _alert.SendTextMessage(systemDeviceMonitor, "Monitor starting");

            while (!_stop)
            {
                foreach (var deviceMonitor in _configuration.ApplicationConfiguration.DeviceMonitors)
                {
                    var presenceRecordTask = _dataStore.GetLatestPresence(
                        deviceMonitor.DeviceGroupId,
                        deviceMonitor.DeviceId,
                        deviceMonitor.DataType);

                    var latestStateTask = _deviceStateRepository.GetLatestDeviceStateEntity(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);
                    var deviceLogTask = _deviceLogRepository.GetLatestPresence(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);
                    var latestKeepAliveTask = _deviceLogRepository.GetLatestKeepAlive(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);

                    var keepAliveTask = _keepAliveMonitor.IsKeepaliveWithinSpec(deviceMonitor);

                    Task.WaitAll(presenceRecordTask, latestKeepAliveTask, deviceLogTask, latestStateTask);

                    var latestState = latestStateTask.Result;
                    var deviceLog = deviceLogTask.Result;
                    var latestKeepAlive = latestKeepAliveTask.Result;

                    var keepAlive = keepAliveTask.Result;
                    var presenceRecord = presenceRecordTask.Result;

                    if (!keepAlive)
                    {
                        var sendKeepAlive = true;
                        if (deviceMonitor.Keepalive.NotifyOnceOnly)
                            sendKeepAlive = (latestState.LastKeepAliveAlert < latestState.LastKnownKeepAlive);

                        if (sendKeepAlive)
                            _alert.SendKeepaliveMissingAlert(deviceMonitor);
                    }

                    var failedTimeInterval = _stateCheckMonitor.GetFailedTimerInterval(deviceMonitor.TimeIntervals, presenceRecord);
                    if (failedTimeInterval != null)
                    {
                        _logger.Info(
                            $"SendTimerIntervalAlert {presenceRecord}, {failedTimeInterval}, {deviceMonitor}");
                        _alert.SendTimerIntervalAlert(presenceRecord, failedTimeInterval, deviceMonitor);
                    }


                    if (deviceMonitor.StateChangeNotification.OnEveryChange)
                    {
                        _logger.Info("State change");
                        var stateChange = _stateChange.GetStateChangeNotification(
                            latestState, deviceLog,
                            deviceMonitor.StateChangeNotification);
                        if (stateChange != null)
                        {
                            _alert.SendStateChangeAlert(stateChange, deviceMonitor);
                        }
                    }

                    var stateOnChange = _stateChange.GetStateChangeFromToNotification(
                        latestState, deviceLog,
                        deviceMonitor.StateChangeNotification);
                    if (stateOnChange != null)
                    {
                        _logger.Info("_alert.SendStateChangeAlert");
                        _alert.SendStateChangeAlert(stateOnChange, deviceMonitor);
                    }

                    latestState.LastKnownDataValue = deviceLog.DataValue;
                    latestState.LastExecutedKeepaliveCheckDate = System.DateTime.Now;
                    latestState.LastKnownDataValueDate = latestKeepAlive.DateTime;
                    if (latestKeepAlive.DateTime > latestState.LastKnownKeepAlive)
                    {
                        latestState.LastKnownKeepAlive = latestKeepAlive.DateTime;
                    }

                    _deviceStateRepository.SetDeviceStateEntity(latestState);
                }

                var count = 0;
                while (_configuration.ApplicationConfiguration.MonitorConfiguration.PollingIntervalSeconds > count)
                {
                    Thread.Sleep(1000);
                    count++;
                    if (_stop) break;
                }
            }

            return 1;
        }
    }
}
