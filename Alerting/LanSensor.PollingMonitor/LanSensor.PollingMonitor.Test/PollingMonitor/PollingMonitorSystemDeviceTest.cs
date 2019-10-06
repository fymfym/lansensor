using System.Collections.Generic;
using FakeItEasy;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Monitor.KeepAlive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.PollingMonitor.Services.Pause;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceState;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.PollingMonitor
{
    public class PollingMonitorSystemDeviceTest
    {
        [Fact]
        public void Monitor_StoppedBeforeRun_PauseIsNotCalled()
        {
            var config = A.Fake<IConfiguration>();
            var alert = A.Fake<IAlert>();
            var stateCheckMonitor = A.Fake<ITimeIntervalMonitor>();
            var keepAliveMonitor = A.Fake<IKeepAliveMonitor>();
            var stateChange = A.Fake<IStateChangeMonitor>();
            var deviceStateRepository = A.Fake<IDeviceStateRepository>();
            var deviceLogRepository = A.Fake<IDeviceLogRepository>();
            var logger = A.Fake<ILogger>();
            var pauseService = A.Fake<IPauseService>();

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
                                    ReceiverId = "slack channel",
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

            var monitor = new Services.Monitor.PollingMonitor(
                config,
                alert,
                stateCheckMonitor,
                keepAliveMonitor,
                stateChange,
                deviceStateRepository,
                deviceLogRepository,
                logger,
                pauseService
                );

            monitor.RunThroughDeviceMonitors();
        }
    }
}
