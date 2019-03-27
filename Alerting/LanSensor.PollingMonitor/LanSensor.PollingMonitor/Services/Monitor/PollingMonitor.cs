using System;
using System.Threading;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor.DataValueToOld;
using LanSensor.PollingMonitor.Services.Monitor.Keepalive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceState;

namespace LanSensor.PollingMonitor.Services.Monitor
{
    public class PollingMonitor : IPollingMonitor
    {
        private readonly IConfiguration _configuration;
        private readonly IAlert _alert;
        private readonly ITimeIntervalMonitor _stateCheckMonitor;
        private readonly IKeepaliveMonitor _keepaliveMonitor;
        private readonly IStateChangeMonitor _stateChange;
        private readonly IDeviceStateRepository _deviceStateRepository;
        private readonly IDeviceLogRepository _deviceLogRepository;
        private readonly IGetDateTime _getDateTime;
        private readonly IDataValueToOldMonitor _dataValueToOldMonitor;
        private bool _stop;

        public PollingMonitor
        (
            IConfiguration configuration,
            IDeviceLogRepository deviceLogRepository,
            IAlert alert,
            ITimeIntervalMonitor stateCheckMonitor,
            IKeepaliveMonitor keepaliveMonitor,
            IStateChangeMonitor stateChange,
            IDeviceStateRepository deviceStateRepository,
            IGetDateTime getDateTime,
            IDataValueToOldMonitor dataValueToOldMonitor
        )
        {
            _deviceLogRepository = deviceLogRepository;
            _deviceStateRepository = deviceStateRepository;
            _configuration = configuration ?? throw new Exception("Add configurtation");
            _alert = alert;
            _stateCheckMonitor = stateCheckMonitor;
            _keepaliveMonitor = keepaliveMonitor;
            _stateChange = stateChange;
            _getDateTime = getDateTime;
            _stop = false;
            _dataValueToOldMonitor = dataValueToOldMonitor;
        }

        public bool StoppedIntentionaly => _stop;

        public void Stop()
        {
            _stop = true;
        }

        public async Task<int> Run()
        {
            while (!_stop)
            {

                foreach (var deviceMonitor in _configuration.ApplicationConfiguration.DeviceMonitors)
                {
                    var presenceRecord = await _deviceLogRepository.GetLatestPresence(
                        deviceMonitor.DeviceGroupId,
                        deviceMonitor.DeviceId,
                        deviceMonitor.DataType);

                    var latestState = await _deviceStateRepository.GetLatestDeviceStateEntity(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);
                    var deviceLog = await _deviceLogRepository.GetLatestPresence(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);
                    var deviceKeepalive = await _deviceLogRepository.GetLatestKeepalive(deviceMonitor.DeviceGroupId, deviceMonitor.DeviceId);

                    latestState.LastExecutedKeepaliveCheckDate = _getDateTime.Now;
                    var keepalive = await _keepaliveMonitor.IsKeepaliveWithinSpec(deviceMonitor);
                    if (!keepalive)
                    {
                        var sendKeepalive = true;
                        if (deviceMonitor.Keepalive.NotifyOnceOnly)
                            sendKeepalive = (latestState.LastKeepAliveAlert < latestState.LastKnownKeepAliveDate);

                        if (sendKeepalive)
                        {
                            _alert.SendKeepaliveMissingAlert(deviceMonitor);
                            latestState.LastKeepAliveAlert = _getDateTime.Now;
                        }
                    }

                    var failedTimeInterval = _stateCheckMonitor.GetFailedTimerInterval(deviceMonitor.TimeIntervals, presenceRecord);
                    if (failedTimeInterval != null)
                    {
                        _alert.SendTimerIntervalAlert(presenceRecord, failedTimeInterval, deviceMonitor);
                    }


                    if (deviceMonitor.StateChangeNotification.OnEveryChange)
                    {
                        var stateChange = _stateChange.GetStateChangeNotification(
                            latestState, deviceLog,
                            deviceMonitor.StateChangeNotification);
                        if (stateChange != null)
                        {
                            _alert.SendStateChangeAlert(stateChange, deviceMonitor);
                        }
                    }

                    // StateChangeFromToNotification
                    var stateOnChange = _stateChange.GetStateChangeFromToNotification(
                        latestState, deviceLog,
                        deviceMonitor.StateChangeNotification);
                    if (stateOnChange != null)
                    {
                        _alert.SendStateChangeAlert(stateOnChange, deviceMonitor);
                    }

                    // Data Value Too Old
                    if (deviceMonitor.DataValueToOld != null)
                    {
                        var dataValueRecord = await _deviceLogRepository.GetLatestPresence(
                            deviceMonitor.DeviceGroupId,
                            deviceMonitor.DeviceId,
                            deviceMonitor.DataValueToOld.DataValue);

                        if (_dataValueToOldMonitor.IsDataValueToOld(dataValueRecord, deviceMonitor.DataValueToOld)) {
                            _alert.SendDataValueToOld(dataValueRecord, deviceMonitor);
                        }

                    }

                    latestState.LastKnownDataValue = deviceLog.DataValue;
                    latestState.LastKnownDataValueDate = deviceKeepalive.DateTime;

                    await _deviceStateRepository.SetDeviceStateEntity(latestState);
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
