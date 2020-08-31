using System;
using System.Collections.Generic;
using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
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
        private readonly IMonitorTools _fakedMonitorTools;
        private readonly IDateTimeService _fakedDateTimeService;

        public PollingMonitorTest()
        {
            _fakedAlert = A.Fake<IAlertService>();
            _fakedDeviceStateService = A.Fake<IDeviceStateService>();
            _fakedLogger = A.Fake<ILogger>();
            _fakedMonitorExecuterList = A.Fake<IEnumerable<IMonitorExecuter>>();
            _fakedPauseService = A.Fake<IPauseService>();
            _fakedMonitorTools = A.Fake<IMonitorTools>();
            _fakedDateTimeService = A.Fake<IDateTimeService>();
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
                _fakedPauseService,
                _fakedMonitorTools,
                _fakedDateTimeService));
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
                _fakedPauseService,
                _fakedMonitorTools,
                _fakedDateTimeService)
                );
        }

        [Fact]
        public void RunConfigurationFileTest()
        {
            IReadEnvironmentService readEnv = A.Fake<IReadEnvironmentService>();

            A.CallTo(() => readEnv.GetEnvironmentVariable(
                    A<string>.Ignored))
                .Returns("http://127.0.0.1");

            var config = new ServiceConfiguration(readEnv);
            var monitor = new Application.Services.PollingMonitor.PollingMonitor(
                config,
                _fakedAlert,
                _fakedDeviceStateService,
                _fakedMonitorExecuterList,
                _fakedLogger,
                _fakedPauseService,
                _fakedMonitorTools,
                _fakedDateTimeService
                );
            Assert.NotNull(monitor);
        }

        [Fact]
        public void RunConfigurationFileRunTest()
        {
            var config = A.Fake<IServiceConfiguration>();

            var monitor = new Application.Services.PollingMonitor.PollingMonitor(
                config,
                _fakedAlert,
                _fakedDeviceStateService,
                _fakedMonitorExecuterList,
                _fakedLogger,
                _fakedPauseService,
                _fakedMonitorTools,
                _fakedDateTimeService
                );
            monitor.Stop();

            Assert.NotNull(monitor);
            Assert.True(monitor.StoppedIntentionally);
        }
    }
}
