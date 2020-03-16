using System;
using System.Configuration;
using FakeItEasy;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Monitor.KeepAlive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.PollingMonitor.Services.Pause;
using LanSensor.Repository;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceState;
using NLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.PollingMonitor
{
    public class PollingMonitorTest
    {
        private readonly IServiceConfiguration _fakedConfig;
        private readonly IDeviceLogRepository _fakedDeviceLogRepository;
        private readonly IAlert _fakedAlert;
        private readonly ILogger _fakedLogger;
        private readonly ITimeIntervalMonitor _fakedStateCheckComparer;
        private readonly IKeepAliveMonitor _fakedKeepAliveMonitor;
        private readonly IStateChangeMonitor _fakedStageChange;
        private readonly IDeviceLogRepository _fakedDeviceLog;
        private readonly IDeviceState _fakedDeviceStage;
        private readonly IPauseService _fakedPauseService;

        public PollingMonitorTest()
        {
            _fakedConfig = A.Fake<IServiceConfiguration>();
            _fakedDeviceLogRepository = A.Fake<IDeviceLogRepository>();
            _fakedAlert = A.Fake<IAlert>();
            _fakedStateCheckComparer = A.Fake<ITimeIntervalMonitor>();
            _fakedKeepAliveMonitor = A.Fake<IKeepAliveMonitor>();
            _fakedStageChange = A.Fake<IStateChangeMonitor>();
            _fakedDeviceLog = A.Fake<IDeviceLogRepository>();
            _fakedDeviceStage = A.Fake<IDeviceState>();
            _fakedLogger = A.Fake<ILogger>();
            _fakedPauseService = A.Fake<IPauseService>();
        }


        [Fact]
        public void RunNulParameterTest()
        {
            Assert.Throws<Exception>(() => new Services.Monitor.PollingMonitor(
                null, _fakedAlert, _fakedStateCheckComparer, _fakedKeepAliveMonitor, _fakedStageChange, _fakedDeviceStage, _fakedDeviceLog, _fakedLogger, _fakedPauseService));
        }

        [Fact]
        public void RunConfigurationFakedParameterTest()
        {
            var monitor = new Services.Monitor.PollingMonitor(
                _fakedConfig, _fakedAlert, _fakedStateCheckComparer, _fakedKeepAliveMonitor, _fakedStageChange, _fakedDeviceStage, _fakedDeviceLog, _fakedLogger, _fakedPauseService);
            Assert.NotNull(monitor);
        }

        [Fact]
        public void RunConfigurationFileTest()
        {
            var config = new ServiceConfiguration(null);
            var monitor = new Services.Monitor.PollingMonitor(
                config, _fakedAlert, _fakedStateCheckComparer, _fakedKeepAliveMonitor, _fakedStageChange, _fakedDeviceStage, _fakedDeviceLog, _fakedLogger, _fakedPauseService);
            Assert.NotNull(monitor);
        }

        [Fact]
        public void RunConfigurationFileRunTest()
        {
            var config = new ServiceConfiguration(null);

            var monitor = new Services.Monitor.PollingMonitor(
                config, _fakedAlert, _fakedStateCheckComparer, _fakedKeepAliveMonitor, _fakedStageChange, _fakedDeviceStage, _fakedDeviceLog, _fakedLogger, _fakedPauseService);
            monitor.Stop();

            Assert.NotNull(monitor);
            Assert.True(monitor.StoppedIntentionally);
        }
    }
}
