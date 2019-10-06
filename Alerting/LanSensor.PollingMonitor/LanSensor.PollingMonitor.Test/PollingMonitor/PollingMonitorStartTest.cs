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
    public class PollingMonitorStartTest
    {
        [Fact]
        public void Monitor_StoppedBeforeRun_PauseIsNotCalled()
        {
            var config = A.Fake<IConfiguration>();
            var dataStore = A.Fake<IDeviceLogRepository>();
            var alert = A.Fake<IAlert>();
            var stateCheckMonitor = A.Fake<ITimeIntervalMonitor>();
            var keepAliveMonitor = A.Fake<IKeepaliveMonitor>();
            var stateChange = A.Fake<IStateChangeMonitor>();
            var deviceStateRepository = A.Fake<IDeviceStateRepository>();
            var deviceLogRepository = A.Fake<IDeviceLogRepository>();
            var logger = A.Fake<ILogger>();
            var pauseService = A.Fake<IPauseService>();

            A.CallTo(() => config.ApplicationConfiguration).Returns(
                new ApplicationConfiguration
                {
                    DeviceMonitors = new List<DeviceMonitor>(),
                    MonitorConfiguration = new MonitorConfiguration
                    {
                        PollingIntervalSeconds = 10
                    }
                }
                );

            var monitor = new Services.Monitor.PollingMonitor(
                config,
                dataStore,
                alert,
                stateCheckMonitor,
                keepAliveMonitor,
                stateChange,
                deviceStateRepository,
                deviceLogRepository,
                logger,
                pauseService
                );

            monitor.Stop();
            monitor.RunInLoop();
            A.CallTo(() => pauseService.Pause(A<int>.Ignored)).MustNotHaveHappened();
        }
    }
}
