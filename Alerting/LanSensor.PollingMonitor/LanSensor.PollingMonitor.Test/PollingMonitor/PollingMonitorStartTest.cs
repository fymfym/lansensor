using System.Collections.Generic;
using FakeItEasy;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Pause;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.PollingMonitor
{
    public class PollingMonitorStartTest
    {
        [Fact]
        public void Monitor_StoppedBeforeRun_PauseIsNotCalled()
        {
            var config = A.Fake<IServiceConfiguration>();
            var alert = A.Fake<IAlert>();
            var deviceStateService = A.Fake<IDeviceStateService>();
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

            var list = new List<IMonitorExecuter>();

            var monitor = new Services.Monitor.PollingMonitor(
                config,
                alert,
                deviceStateService,
                list,
                logger,
                pauseService
                );

            monitor.Stop();
            monitor.RunInLoop();
            A.CallTo(() => pauseService.Pause(A<int>.Ignored)).MustNotHaveHappened();
        }
    }
}
