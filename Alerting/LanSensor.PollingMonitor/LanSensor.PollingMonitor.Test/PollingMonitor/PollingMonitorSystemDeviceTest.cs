using System.Collections.Generic;
using FakeItEasy;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.PollingMonitor
{
    public class PollingMonitorSystemDeviceTest
    {
        [Fact]
        public void Monitor_StoppedBeforeRun_PauseIsNotCalled()
        {
            var config = A.Fake<IServiceConfiguration>();
            var alert = A.Fake<IAlertService>();
            var deviceStateService = A.Fake<IDeviceStateService>();
            var logger = A.Fake<ILogger>();
            var pauseService = A.Fake<IPauseService>();
            var fakedMonitorTools = A.Fake<IMonitorTools>();
            var fakedDateTimeService = A.Fake<IDateTimeService>();

            A.CallTo(() => config.ApplicationConfiguration).Returns(
                new ApplicationConfiguration
                {
                    DeviceMonitors = new List<DeviceMonitor>
                    {
                        new DeviceMonitor
                        {
                            DeviceGroupId = "system",
                            MessageMediums = new List<MessageMedium>
                            {
                                new MessageMedium
                                {
                                    MediumType = "slack",
                                    Message = "some message"
                                }
                            }
                        }
                    },
                    MonitorConfiguration = new MonitorConfiguration
                    {
                        PollingIntervalSeconds = 1
                    }
                }
                );

            var monitor = new Application.Services.PollingMonitor.PollingMonitor(
                config,
                alert,
                deviceStateService,
                new List<IMonitorExecuter>(),
                logger,
                pauseService,
                fakedMonitorTools,
                fakedDateTimeService
                );

            monitor.RunThroughDeviceMonitors();
        }
    }
}
