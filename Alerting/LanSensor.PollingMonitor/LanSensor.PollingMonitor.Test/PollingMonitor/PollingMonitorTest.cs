using System;
using System.Collections.Generic;
using FakeItEasy;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Services.Pause;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.PollingMonitor
{
    public class PollingMonitorTest
    {
        private readonly IAlertService _fakedAlert;
        private readonly IPauseService _fakedPauseService;
        private readonly ILogger _fakedLogger;
        private readonly IDeviceStateService _fakedDeviceStateService;
        private readonly IEnumerable<IMonitorExecuter> _fakedMonitorExecuterList;

        public PollingMonitorTest()
        {
            _fakedAlert = A.Fake<IAlertService>();
            _fakedDeviceStateService = A.Fake<IDeviceStateService>();
            _fakedLogger = A.Fake<ILogger>();
            _fakedMonitorExecuterList = A.Fake<IEnumerable<IMonitorExecuter>>();
            _fakedPauseService = A.Fake<IPauseService>();
        }


        [Fact]
        public void RunNulParameterTest()
        {
            Assert.Throws<Exception>(() => new Application.Services.PollingMonitor.PollingMonitor(
                null,
                _fakedAlert,
                _fakedDeviceStateService,
                _fakedMonitorExecuterList,
                _fakedLogger,
                _fakedPauseService));
        }

        [Fact]
        public void RunConfigurationFakedParameterTest()
        {
            Assert.Throws<Exception>(() =>
            new Application.Services.PollingMonitor.PollingMonitor(
                null,
                _fakedAlert,
                _fakedDeviceStateService,
                _fakedMonitorExecuterList,
                _fakedLogger,
                _fakedPauseService)
                );
        }

        [Fact]
        public void RunConfigurationFileTest()
        {
            var config = new ServiceConfiguration(null);
            var monitor = new Application.Services.PollingMonitor.PollingMonitor(
                config,
                _fakedAlert,
                _fakedDeviceStateService,
                _fakedMonitorExecuterList,
                _fakedLogger,
                _fakedPauseService);
            Assert.NotNull(monitor);
        }

        [Fact]
        public void RunConfigurationFileRunTest()
        {
            var config = new ServiceConfiguration(null);

            var monitor = new Application.Services.PollingMonitor.PollingMonitor(
                config,
                _fakedAlert,
                _fakedDeviceStateService,
                _fakedMonitorExecuterList,
                _fakedLogger,
                _fakedPauseService);
            monitor.Stop();

            Assert.NotNull(monitor);
            Assert.True(monitor.StoppedIntentionally);
        }
    }
}
