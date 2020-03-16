using System.Collections.Generic;
using System.Linq;
using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using NLog;
using SlackAPI;

namespace LanSensor.PollingMonitor.Services.Alert.Slack
{
    public class SendSlackAlert : IAlert
    {
        private readonly ILogger _logger;
        private IList<Channel> _slackAlertChannels;
        private readonly SlackClient _slackClient;

        public SendSlackAlert(
            IServiceConfiguration configuration,
            ILogger logger)
        {
            _logger = logger;
            var apiKey = configuration.ApplicationConfiguration.SlackConfiguration.ApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.Warn("Slack not initialized");
            }

            _slackClient = new SlackClient(apiKey);
            _slackClient.GetChannelList(SlackChannelListResponse);
        }


        private void SlackChannelListResponse(ChannelListResponse obj)
        {
            if (obj?.channels == null)
            {
                _logger.Warn("Slack channel list not obtainable");
                return;
            }

            _slackAlertChannels = obj.channels.ToList();
        }

        public bool SendStateChangeAlert(StateChangeResult stateNotification, DeviceMonitor deviceMonitor)
        {
            var deviceMonitorMessage = GetMonitorMessage(deviceMonitor);
            var msg =
                $"State changed for *{deviceMonitor.DeviceGroupId}* / *{deviceMonitor.DeviceId}*, with message *{deviceMonitorMessage}*";
            return SendToSlack(deviceMonitor, msg);
        }

        public bool SendKeepAliveMissingAlert(DeviceMonitor deviceMonitor)
        {
            var deviceMonitorMessage = GetMonitorMessage(deviceMonitor);
            var msg =
                $"KeepAlive missing for *{deviceMonitor.DeviceGroupId}* / *{deviceMonitor.DeviceId}*, with message *{deviceMonitorMessage}*";
            _logger.Info($"Slack SendKeepAliveMissingAlert:{msg}");
            return SendToSlack(deviceMonitor, msg);
        }

        public bool SendTimerIntervalAlert(DeviceLogEntity presenceRecord, TimeInterval timeInterval, DeviceMonitor deviceMonitor)
        {
            var deviceMonitorMessage = GetMonitorMessage(deviceMonitor);
            var msg =
                $"Device *{deviceMonitor.DeviceGroupId}* / *{deviceMonitor.DeviceId}* is outside desired value of *{timeInterval.DataValue}* with message *{deviceMonitorMessage}*";
            _logger.Info($"Slack SendTimerIntervalAlert:{msg}");
            return SendToSlack(deviceMonitor, msg);
        }

        public bool SendTextMessage(DeviceMonitor deviceMonitor, string message)
        {
            _logger.Info($"Slack SendTextMessage:{message}");
            return SendToSlack(deviceMonitor, message);
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
            if (_slackAlertChannels == null)
            {
                _logger.Error("Slack not properly configured");
                return null;
            }

            var channel = _slackAlertChannels.FirstOrDefault(x => x.name == messageMedium.ReceiverId);
            return channel;
        }

        private static string GetMonitorMessage(DeviceMonitor monitor)
        {
            var messageMedium = monitor.MessageMediums.FirstOrDefault(x => x.MediumType.ToLower().Trim() == "slack");
            return messageMedium?.Message;
        }
    }
}
