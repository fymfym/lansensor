using System.Collections.Generic;
using FakeItEasy;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Services.Monitor;
using LanSensor.PollingMonitor.Services.Pause;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class PollingMonitorRunThroughMonitors
    {
        private readonly ILogger _fakedLogger;
        private readonly IPauseService _fakedPauseService;
        private IMonitorExecuter _fakedMonitor1;
        private IMonitorExecuter _fakedMonitor2;
        private readonly IDeviceStateService _fakedDeviceStateService;
        private readonly IAlertService _fakedAlertService;

        public PollingMonitorRunThroughMonitors()
        {
            _fakedLogger = A.Fake<ILogger>();
            _fakedPauseService = A.Fake<IPauseService>();
            _fakedDeviceStateService = A.Fake<IDeviceStateService>();
            _fakedMonitor1 = A.Fake<IMonitorExecuter>();
            _fakedMonitor2 = A.Fake<IMonitorExecuter>();
            _fakedAlertService = A.Fake<IAlertService>();
        }

        [Fact]
        public void PollingMonitorRun_TwoMonitors_BothValidatedForRun()
        {
            var config = A.Fake<IServiceConfiguration>();
            A.CallTo(() => config.ApplicationConfiguration).Returns(GetApplicationConfiguration());

            IPollingMonitor pollingMonitor = CreatePollingMonitor();

            pollingMonitor.RunThroughDeviceMonitors();
            pollingMonitor.Stop();

            A.CallTo(() => _fakedAlertService.SendKeepAliveMissingAlert(A<DeviceMonitor>.Ignored)).MustNotHaveHappened();

            A.CallTo(() => _fakedDeviceStateService.GetDeviceState(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened();
            A.CallTo(() => _fakedDeviceStateService.SaveDeviceState(A<DeviceStateEntity>.Ignored)).MustHaveHappened();
            A.CallTo(() => _fakedMonitor1.CanMonitorRun(A<DeviceMonitor>.Ignored)).MustHaveHappened();
            A.CallTo(() => _fakedMonitor2.CanMonitorRun(A<DeviceMonitor>.Ignored)).MustHaveHappened();
            A.CallTo(() => _fakedMonitor1.Run(A<DeviceStateEntity>.Ignored, A<DeviceMonitor>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _fakedMonitor2.Run(A<DeviceStateEntity>.Ignored, A<DeviceMonitor>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void PollingMonitorRun_TwoMonitors_OneRun()
        {
            IPollingMonitor pollingMonitor = CreatePollingMonitor();

            A.CallTo(() => _fakedMonitor1.CanMonitorRun(A<DeviceMonitor>.Ignored)).Returns(true);

            pollingMonitor.RunThroughDeviceMonitors();
            pollingMonitor.Stop();

            A.CallTo(() => _fakedDeviceStateService.GetDeviceState(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened();
            A.CallTo(() => _fakedDeviceStateService.SaveDeviceState(A<DeviceStateEntity>.Ignored)).MustHaveHappened();
            A.CallTo(() => _fakedMonitor1.CanMonitorRun(A<DeviceMonitor>.Ignored)).MustHaveHappened();
            A.CallTo(() => _fakedMonitor2.CanMonitorRun(A<DeviceMonitor>.Ignored)).MustHaveHappened();
            A.CallTo(() => _fakedMonitor1.Run(A<DeviceStateEntity>.Ignored, A<DeviceMonitor>.Ignored)).MustHaveHappened();
            A.CallTo(() => _fakedMonitor2.Run(A<DeviceStateEntity>.Ignored, A<DeviceMonitor>.Ignored)).MustNotHaveHappened();
        }


        private Application.Services.PollingMonitor.PollingMonitor CreatePollingMonitor()
        {
            var config = A.Fake<IServiceConfiguration>();
            A.CallTo(() => config.ApplicationConfiguration).Returns(GetApplicationConfiguration());

            return new Application.Services.PollingMonitor.PollingMonitor(
                config,
                _fakedAlertService,
                _fakedDeviceStateService,
                GetMonitorList(),
                _fakedLogger,
                _fakedPauseService);
        }

        private IEnumerable<IMonitorExecuter> GetMonitorList()
        {
            _fakedMonitor1 = A.Fake<IMonitorExecuter>();
            _fakedMonitor2 = A.Fake<IMonitorExecuter>();

            return new List<IMonitorExecuter>
            {
                _fakedMonitor1,
                _fakedMonitor2
            };
        }

        private static ApplicationConfiguration GetApplicationConfiguration()
        {
            return new ApplicationConfiguration
            {
                DeviceMonitors = new[]
                {
                    new DeviceMonitor
                    {
                        DeviceGroupId = "dg",
                        DeviceId = "di",
                    },
                },
                MonitorConfiguration = new MonitorConfiguration
                {
                    PollingIntervalSeconds = 1
                }
            };
        }
    }
}
