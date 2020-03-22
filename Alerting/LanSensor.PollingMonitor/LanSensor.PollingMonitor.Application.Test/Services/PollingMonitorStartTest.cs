using System.Collections.Generic;
using FakeItEasy;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Application.Test.Services
{
    public class PollingMonitorStartTest
    {
        [Fact]
        public void Monitor_StoppedBeforeRun_PauseIsNotCalled()
        {
            var config = A.Fake<IServiceConfiguration>();
            var alert = A.Fake<IAlertService>();
            var deviceStateService = A.Fake<IDeviceStateService>();
            var logger = A.Fake<ILogger>();
            var pauseService = A.Fake<IPauseService>();
            var monitorTools = A.Fake<IMonitorTools>();
            var dateTimeService = A.Fake<IDateTimeService>();

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

            var monitor = new Application.Services.PollingMonitor.PollingMonitor(
                config,
                alert,
                deviceStateService,
                new List<IMonitorExecuter>(),
                logger,
                pauseService,
                monitorTools,
                dateTimeService
                );

            monitor.Stop();
            monitor.RunInLoop();
            A.CallTo(() => pauseService.Pause(A<int>.Ignored)).MustNotHaveHappened();
        }
    }
}
