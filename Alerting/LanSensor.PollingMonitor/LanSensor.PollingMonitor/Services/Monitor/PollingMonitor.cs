using System;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Monitor.Keepalive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor
{
    public class PollingMonitor : IPollingMonitor
    {
        private readonly IConfiguration _configuration;
        private readonly IDeviceLogRepository _dastastore;
        private readonly IAlert _alert;
        private readonly ITimeIntervalMonitor _stateCheckMonitor;
        private readonly IKeepaliveMonitor _keepaliveMonitor;
        private readonly IStateChangeMonitor _stateChange;
        private bool _stop;

        public PollingMonitor
        (
            IConfiguration configuration,
            IDeviceLogRepository dastastore,
            IAlert alert,
            ITimeIntervalMonitor stateCheckMonitor,
            IKeepaliveMonitor keepaliveMonitor,
            IStateChangeMonitor stateChange
        )
        {
            _configuration = configuration ?? throw new Exception("Add configurtation");
            _dastastore = dastastore;
            _alert = alert;
            _stateCheckMonitor = stateCheckMonitor;
            _keepaliveMonitor = keepaliveMonitor;
            _stateChange = stateChange;
            _stop = false;
        }

        public bool StoppedIntentionaly => _stop;

        public void Stop()
        {
            _stop = true;
        }

        public async Task<int> Run()
        {
            while(!_stop)
            {

                foreach (var deviceMonitor in _configuration.ApplicationConfiguration.DeviceMonitors)
                {

                    var keepalive = await _keepaliveMonitor.IsKeepaliveWithinSpec(deviceMonitor);
                    if (!keepalive)
                        _alert.SendKeepaliveMissingAlert(deviceMonitor);

                    var presenceRecord = await _dastastore.GetLatestPresence(deviceMonitor.DeviceGroupId, 
                        deviceMonitor.DeviceId, 
                        deviceMonitor.DataType );

                    var failedTimeInterval = _stateCheckMonitor.GetFailedTimerInterval(deviceMonitor.TimeIntervals, presenceRecord);
                    if (failedTimeInterval != null)
                    {
                        _alert.SendTimerIntervalAlert(presenceRecord,failedTimeInterval,deviceMonitor);
                    }

                    var stateChange = await _stateChange.GetStateChangeNotification(
                        deviceMonitor.DeviceGroupId,
                        deviceMonitor.DeviceId, 
                        deviceMonitor.StateChangeNotification );
                    if (stateChange != null)
                    {
                        _alert.SendStateChangeAlert(stateChange, deviceMonitor);
                    }

                }

            }

            return _stop ? 0: -1;
        }
        
    }
}
