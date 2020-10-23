using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.Alert.Slack;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Infrastructure.RestServices;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Slack
{
    public class SendSlackAlertTest
    {
        private readonly IServiceConfiguration _fakedConfiguration;
        private readonly IMessageSender _fakedMessageSender;
        private readonly ILogger _fakedLogger;

        public SendSlackAlertTest()
        {
            _fakedConfiguration = A.Fake<IServiceConfiguration>();
            _fakedMessageSender = A.Fake<IMessageSender>();
            _fakedLogger = A.Fake<ILogger>();
        }

        [Fact]
        public void SendSlackAlert_WithoutMonitor()
        {
            var service = new SendSlackAlertService(_fakedConfiguration, _fakedMessageSender, _fakedLogger);
            service.SendTextMessage(null, "Test #1");

            A.CallTo(() => _fakedMessageSender.SendMessage(A<string>.That.Matches(x => x == "Test #1 - (Monitor less)")))
                .MustHaveHappened();
        }

        [Fact]
        public void SendSlackAlert_WithMonitor()
        {
            var monitor = new DeviceMonitor
            {
                Name = "Monitor #1"
            };

            var service = new SendSlackAlertService(_fakedConfiguration, _fakedMessageSender, _fakedLogger);
            service.SendTextMessage(monitor, "Test #2");

            A.CallTo(() => _fakedMessageSender.SendMessage(A<string>.That.Matches(x => x == "Test #2 - Monitor #1")))
                .MustHaveHappened();
        }
    }
}
