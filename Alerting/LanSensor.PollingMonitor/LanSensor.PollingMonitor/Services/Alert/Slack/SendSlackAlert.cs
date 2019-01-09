using System;
using System.Linq;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;
using LanSensor.Models.DeviceState;

namespace LanSensor.PollingMonitor.Services.Alert.Slack
{
    public class SendSlackAlert : IAlert
    {

        private readonly IConfiguration _configuration;

        public SendSlackAlert(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendStateChangeAlert(StateChangeResult stateNotification, DeviceMonitor deviceMonitor)
        {
            //TODO - Implement Send statechange Alert
            throw new NotImplementedException();
        }

        public bool SendKeepaliveMissingAlert(DeviceMonitor deviceMonitor)
        {
            //TODO - Implement Send keepalive missing alert
            throw new NotImplementedException();
        }

        public bool SendTimerIntervalAlert(DeviceLogEntity presenceRecord, TimeInterval timeInterval, DeviceMonitor deviceMonitor)
        {
            if (timeInterval == null) throw new ArgumentNullException(nameof(timeInterval));
            var message = timeInterval.MessageMediums.FirstOrDefault(x => x.MediumType.ToLower() == "slack") ??
                          deviceMonitor?.MessageMediums?.FirstOrDefault(x => x.MediumType.ToLower() == "slack");
            if (message == null) throw new Exception("no deviceMonitors/messageMediums/mediumType or deviceMonitors/messageMediums/timeInterval[]/mediumType has value 'slack'");

            var msg = 
                $"device {deviceMonitor.DeviceGroupId} / {deviceMonitor.DeviceId} is outside desired value of '{timeInterval.DataValue}' with message {timeInterval.AlertMessage}";

            return SendToSlack(msg);
        }

        private bool SendToSlack(string message)
        {
            //TODO - Implement Slack send 
            var slackApiKey = _configuration.ApplicationConfiguration.SlackConfiguration.ApiKey;
            return false;
        }
    }
}
