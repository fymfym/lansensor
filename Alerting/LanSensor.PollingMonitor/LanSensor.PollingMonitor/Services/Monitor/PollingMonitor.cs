using System;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Monitor.Keepalive;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;

namespace LanSensor.PollingMonitor.Services.Monitor
{
    public class PollingMonitor : IPollingMonitor
    {
        private readonly IConfiguration _configuration;
        private readonly IDeviceLogRepository _dastastore;
        private readonly IAlert _alert;
        private readonly ITimeIntervalComparer _stateCheckComparer;
        private readonly IKeepaliveMonitor _keepalive;
        private bool _stop;

        public PollingMonitor
        (
            IConfiguration configuration,
            IDeviceLogRepository dastastore,
            IAlert alert,
            ITimeIntervalComparer stateCheckComparer,
            IKeepaliveMonitor keepalive
        )
        {
            _configuration = configuration ?? throw new Exception("Add configurtation");
            _dastastore = dastastore;
            _alert = alert;
            _stateCheckComparer = stateCheckComparer;
            _keepalive = keepalive;
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

                    var keepalive = await _keepalive.IsKeepaliveWithinSpec(deviceMonitor);
                    if (!keepalive)
                        _alert.SendKeepaliveMissing(deviceMonitor);

                    var presenceRecord = await _dastastore.GetLatestPresence(deviceMonitor.DeviceGroupId, 
                        deviceMonitor.DeviceId, 
                        deviceMonitor.DataType );

                    var failedTimeInterval = _stateCheckComparer.GetFailedTimerInterval(deviceMonitor.TimeIntervals, presenceRecord);
                    if (failedTimeInterval != null)
                    {
                        _alert.SendAlert(presenceRecord,failedTimeInterval,deviceMonitor);
                    }
                }

            }

            return _stop ? 0: -1;
        }
        
    }
}
