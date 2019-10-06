using System;
using System.Linq;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;
using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Monitor.KeepAlive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.PollingMonitor.Services.Pause;
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
        private readonly IPauseService _pauseService;

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
            ILogger logger,
            IPauseService pauseService
        )
        {
            _deviceLogRepository = deviceLogRepository;
            _logger = logger;
            _pauseService = pauseService;
            _deviceStateRepository = deviceStateRepository;
            _configuration = configuration ?? throw new Exception("Add configurtation");
            _dataStore = dataStore;
            _alert = alert;
            _stateCheckMonitor = stateCheckMonitor;
            _keepAliveMonitor = keepAliveMonitor;
            _stateChange = stateChange;
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

                _logger.Info($"Device monitor: {deviceMonitor.DeviceGroupId}, {deviceMonitor.DeviceId}");

                var presenceRecordTask = _dataStore.GetLatestPresence(
                    deviceMonitor.DeviceGroupId,
                    deviceMonitor.DeviceId,
                    deviceMonitor.DataType);

                var latestStateTask = _deviceStateRepository.GetLatestDeviceStateEntity(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);
                var deviceLogTask = _deviceLogRepository.GetLatestPresence(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);
                var latestKeepAliveTask = _deviceLogRepository.GetLatestKeepAlive(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);

                var keepAliveTask = _keepAliveMonitor.IsKeepAliveWithinSpec(deviceMonitor);

                Task.WaitAll(presenceRecordTask, latestKeepAliveTask, deviceLogTask, latestStateTask);

                var latestState = latestStateTask.Result ?? new DeviceStateEntity();

                var deviceLog = deviceLogTask.Result ?? new DeviceLogEntity();

                var latestKeepAlive = latestKeepAliveTask.Result;
                if (latestKeepAlive == null)
                {
                    continue;
                }

                var keepAlive = keepAliveTask.Result;
                var presenceRecord = presenceRecordTask.Result;

                if (!keepAlive && deviceMonitor.KeepAlive != null)
                {
                    var sendKeepAlive = true;
                    if (deviceMonitor.KeepAlive.NotifyOnceOnly)
                        sendKeepAlive = latestState.LastKeepAliveAlert < latestState.LastKnownKeepAlive;

                    if (sendKeepAlive)
                        _alert.SendKeepAliveMissingAlert(deviceMonitor);
                }

                if (deviceMonitor.TimeIntervals != null)
                {
                    var failedTimeInterval = _stateCheckMonitor.GetFailedTimerInterval(deviceMonitor.TimeIntervals, presenceRecord);
                    if (failedTimeInterval != null)
                    {
                        _logger.Info(
                            $"SendTimerIntervalAlert {presenceRecord}, {failedTimeInterval}, {deviceMonitor}");
                        _alert.SendTimerIntervalAlert(presenceRecord, failedTimeInterval, deviceMonitor);
                    }
                }

                if (deviceMonitor.StateChangeNotification == null) continue;

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

                latestState.DeviceGroupId = deviceMonitor.DeviceGroupId;
                latestState.DeviceId = deviceMonitor.DeviceId;
                latestState.LastKnownDataValue = deviceLog.DataValue;
                latestState.LastExecutedKeepAliveCheckDate = System.DateTime.Now;
                latestState.LastKnownDataValueDate = latestKeepAlive.DateTime;
                if (latestKeepAlive.DateTime > latestState.LastKnownKeepAlive)
                {
                    latestState.LastKnownKeepAlive = latestKeepAlive.DateTime;
                }

                _deviceStateRepository.SetDeviceStateEntity(latestState).Wait();
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
