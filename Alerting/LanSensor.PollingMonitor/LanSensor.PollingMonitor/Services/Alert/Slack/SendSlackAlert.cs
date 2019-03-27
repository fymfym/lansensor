using System;
using System.Collections.Generic;
using System.Linq;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;
using LanSensor.Models.DeviceState;
using SlackAPI;

namespace LanSensor.PollingMonitor.Services.Alert.Slack
{
    public class SendSlackAlert : IAlert
    {
        private IList<Channel> _slackAlertChannels;
        private readonly SlackClient _slackClient;

        public SendSlackAlert(
            IConfiguration configuration)
        {
            _slackClient = new SlackClient(configuration.ApplicationConfiguration.SlackConfiguration.ApiKey);
            _slackClient.GetChannelList(SlackChannelListResponse);
            int count = 1000;
            while (true)
            {
                if (_slackAlertChannels != null) break;
                System.Threading.Thread.Sleep(200);
                if (count-- < 0)
                {
                    throw new Exception("No connection to slack");
                }
            }
        }

        private void SlackChannelListResponse(ChannelListResponse obj)
        {
            _slackAlertChannels = obj.channels.ToList();
        }

        public bool SendStateChangeAlert(StateChangeResult stateNotification, DeviceMonitor deviceMonitor)
        {
            var deviceMonitorMessage = GetMonitorMessage(deviceMonitor);
            var msg =
                $"State changed for {deviceMonitor.DeviceGroupId} / {deviceMonitor.DeviceId}, with message '{deviceMonitorMessage}'";
            return SendToSlack(deviceMonitor, msg);
        }

        public bool SendKeepaliveMissingAlert(DeviceMonitor deviceMonitor)
        {
            var deviceMonitorMessage = GetMonitorMessage(deviceMonitor);
            var msg =
                $"Keepalive missing for {deviceMonitor.DeviceGroupId} / {deviceMonitor.DeviceId}, with message '{deviceMonitorMessage}'";
            return SendToSlack(deviceMonitor, msg);
        }

        public bool SendTimerIntervalAlert(DeviceLogEntity presenceRecord, TimeInterval timeInterval, DeviceMonitor deviceMonitor)
        {
            var deviceMonitorMessage = GetMonitorMessage(deviceMonitor);
            var msg =
                $"device {deviceMonitor.DeviceGroupId} / {deviceMonitor.DeviceId} is outside desired value of '{timeInterval.DataValue}' with message {deviceMonitorMessage}";
            return SendToSlack(deviceMonitor, msg);
        }

        public bool SendDataValueToOld(DeviceLogEntity presenceRecord, DeviceMonitor deviceMonitor)
        {
            var deviceMonitorMessage = GetMonitorMessage(deviceMonitor);
            var msg =
                $"Device {deviceMonitor.DeviceGroupId} / {deviceMonitor.DeviceId} data value is old than {deviceMonitor.DataValueToOld.MaxAgeInMinutes} minutes - message {deviceMonitorMessage}";
            return SendToSlack(deviceMonitor, msg);
        }

        private bool SendToSlack(DeviceMonitor monitor, string message)
        {
            var slackChannel = GetMonitorSlackChannel(monitor);
            if (slackChannel == null) return false;
            _slackClient.PostMessage(null, slackChannel.id, message);
            return true;
        }

        private Channel GetMonitorSlackChannel(DeviceMonitor monitor)
        {
            var messageMedium = monitor.MessageMediums.FirstOrDefault(x => x.MediumType.ToLower().Trim() == "slack");
            if (messageMedium == null) return null;
            var channel = _slackAlertChannels.FirstOrDefault(x => x.name == messageMedium.ReceiverId);
            return channel;
        }

        private string GetMonitorMessage(DeviceMonitor monitor)
        {
            var messageMedium = monitor.MessageMediums.FirstOrDefault(x => x.MediumType.ToLower().Trim() == "slack");
            return messageMedium?.Message;
        }


    }
}
