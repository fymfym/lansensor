using System;
using System.Linq;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;

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

        public bool SendAlert (DeviceLog presenceRecord, TimeInterval timeInterval, DeviceMonitor deviceMonitor)
        {
            if (timeInterval == null) throw new ArgumentNullException(nameof(timeInterval));
            var message = timeInterval.MessageMediums.FirstOrDefault(x => x.MediumType.ToLower() == "slack") ??
                          deviceMonitor?.MessageMediums?.FirstOrDefault(x => x.MediumType.ToLower() == "slack");
            if (message == null) throw new Exception("no deviceMonitors/messageMediums/mediumType or deviceMonitors/messageMediums/timeInterval[]/mediumType has value 'slack'");

            //var msg = 
            //    $"device {deviceMonitor.DeviceGroupId} / {deviceMonitor.DeviceId} is outside desired value of '{timeInterval?.DataValue}' with message {timeInterval.AlertMessage}";

            var slackApiKey =  _configuration.ApplicationConfiguration.SlackConfiguration.ApiKey;

            return string.IsNullOrEmpty(slackApiKey);

        }

        public bool SendKeepaliveMissing(DeviceMonitor deviceMonitor)
        {
            return false;
        }
    }
}
