using System;
using FakeItEasy;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor.DataValueToOld;
using LanSensor.PollingMonitor.Services.Monitor.Keepalive;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceState;
using Xunit;

namespace LanSensor.PollingMonitor.Test
{

    public class PollingMonitorTest
    {
        private readonly IConfiguration _fakedConfig;
        private readonly IDeviceLogRepository _fakedDeviceLogRepository;
        private readonly IAlert _fakedAlert;
        private readonly ITimeIntervalMonitor _fakedStateCheckComparer;
        private readonly IKeepaliveMonitor _fakedKeepaliveMonitor;
        private readonly IStateChangeMonitor _fakedStageChange;
        private readonly IDeviceStateRepository _fakedDeviceStage;
        private readonly IGetDateTime _fakedGetDate;
        private readonly IDataValueToOldMonitor _fakedDataToOldMonitor;


        public PollingMonitorTest()
        {
            _fakedConfig = A.Fake<IConfiguration>();
            _fakedDeviceLogRepository = A.Fake<IDeviceLogRepository>();
            _fakedAlert = A.Fake<IAlert>();
            _fakedStateCheckComparer = A.Fake<ITimeIntervalMonitor>();
            _fakedKeepaliveMonitor = A.Fake<IKeepaliveMonitor>();
            _fakedStageChange = A.Fake<IStateChangeMonitor>();
            _fakedDeviceStage = A.Fake<IDeviceStateRepository>();
            _fakedGetDate = A.Fake<IGetDateTime>();
            _fakedDataToOldMonitor = A.Fake<IDataValueToOldMonitor>();
        }


        [Fact]
        public void RunNulParameterTest()
        {
            Assert.Throws<Exception>(() => new Services.Monitor.PollingMonitor(
                null,_fakedDeviceLogRepository,_fakedAlert,_fakedStateCheckComparer,
                _fakedKeepaliveMonitor, _fakedStageChange, _fakedDeviceStage, _fakedGetDate, _fakedDataToOldMonitor));
        }

        [Fact]
        public void RunConfigurationFakedParameterTest()
        {
            var monitor = new Services.Monitor.PollingMonitor(
                _fakedConfig, _fakedDeviceLogRepository,_fakedAlert,_fakedStateCheckComparer,
                _fakedKeepaliveMonitor, _fakedStageChange, _fakedDeviceStage, _fakedGetDate, _fakedDataToOldMonitor);
            Assert.NotNull(monitor);
        }

        [Fact]
        public void RunConfigurationFileTest()
        {
            var config = new Configuration(null);
            var monitor = new Services.Monitor.PollingMonitor(
                config, _fakedDeviceLogRepository,_fakedAlert,_fakedStateCheckComparer,_fakedKeepaliveMonitor, 
                _fakedStageChange, _fakedDeviceStage, _fakedGetDate, _fakedDataToOldMonitor);
            Assert.NotNull(monitor);
        }

        [Fact]
        public void RunConfigurationFileRunTest()
        {
            var config = new Configuration(null);

            var monitor = new Services.Monitor.PollingMonitor(
                config, _fakedDeviceLogRepository,_fakedAlert,_fakedStateCheckComparer,_fakedKeepaliveMonitor,
                _fakedStageChange, _fakedDeviceStage, _fakedGetDate, _fakedDataToOldMonitor);
            monitor.Stop();

            Assert.NotNull(monitor);
            Assert.True(monitor.StoppedIntentionaly);
        }



    }
}
