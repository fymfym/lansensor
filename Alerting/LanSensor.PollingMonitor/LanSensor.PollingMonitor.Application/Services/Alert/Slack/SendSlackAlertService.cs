using System.Linq;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Infrastructure.RestServices;
using NLog;

namespace LanSensor.PollingMonitor.Application.Services.Alert.Slack
{
    public class SendSlackAlertService : IAlertService
    {
        private readonly IMessageSender _messageSender;
        private readonly ILogger _logger;

        public SendSlackAlertService(
            IServiceConfiguration configuration,
            IMessageSender messageSender,
            ILogger logger)
        {
            _messageSender = messageSender;
            _logger = logger;
            _logger.Info("SendSlackAlertService construct");

            var apiKey = configuration?.ApplicationConfiguration?.SlackConfiguration?.ApiKey;
            var channel = configuration?.ApplicationConfiguration?.SlackConfiguration?.ChannelId;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.Warn($"Slack API key missing value");
            }

            if (string.IsNullOrWhiteSpace(channel))
            {
                _logger.Warn($"Slack channelæ id missing value");
            }
        }

        public bool SendStateChangeAlert(StateChangeResult stateNotification, DeviceMonitor deviceMonitor)
        {
            var deviceMonitorMessage = GetMonitorMessage(deviceMonitor);
            var msg =
                $"{deviceMonitor.Name} / *{deviceMonitor.DeviceGroupId}* / *{deviceMonitor.DeviceId}*: *{deviceMonitorMessage}*";
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

        public bool SendTimerIntervalAlert(DeviceLogEntity presenceRecord, DeviceMonitor deviceMonitor)
        {
            var deviceMonitorMessage = GetMonitorMessage(deviceMonitor);
            var msg =
                $"Device *{deviceMonitor.DeviceGroupId}* / *{deviceMonitor.DeviceId}* is outside desired value, message *{deviceMonitorMessage}*";
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
            _messageSender.SendMessage(message);
            return true;
        }

        private static string GetMonitorMessage(DeviceMonitor monitor)
        {
            var messageMedium = monitor.MessageMediums.FirstOrDefault(x => x.MediumType.ToLower().Trim() == "slack");
            return messageMedium?.Message;
        }
    }
}
