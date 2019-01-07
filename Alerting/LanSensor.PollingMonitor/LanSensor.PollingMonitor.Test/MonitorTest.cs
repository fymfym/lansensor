using System;
using FakeItEasy;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.Monitor.Keepalive;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test
{

    public class PollingMonitorTest
    {
        private readonly IConfiguration _fakedConfig;
        private readonly IDeviceLogRepository _fakedDataStore;
        private readonly IAlert _fakedAlert;
        private readonly ITimeIntervalComparer _fakedStateCheckComparer;
        private readonly IKeepaliveMonitor _fakedKeepaliveMonitor;

        public PollingMonitorTest()
        {
            _fakedConfig = A.Fake<IConfiguration>();
            _fakedDataStore = A.Fake<IDeviceLogRepository>();
            _fakedAlert = A.Fake<IAlert>();
            _fakedStateCheckComparer = A.Fake<ITimeIntervalComparer>();
            _fakedKeepaliveMonitor = A.Fake<IKeepaliveMonitor>();
        }


        [Fact]
        public void RunNulParameterTest()
        {
            Assert.Throws<Exception>(() => new Services.Monitor.PollingMonitor(null,_fakedDataStore,_fakedAlert,_fakedStateCheckComparer,_fakedKeepaliveMonitor));
        }

        [Fact]
        public void RunConfigurationFakedParameterTest()
        {
            var monitor = new Services.Monitor.PollingMonitor(_fakedConfig, _fakedDataStore,_fakedAlert,_fakedStateCheckComparer,_fakedKeepaliveMonitor);
            Assert.NotNull(monitor);
        }

        [Fact]
        public void RunConfigurationFileTest()
        {
            var config = new Configuration(null);
            var monitor = new Services.Monitor.PollingMonitor(config, _fakedDataStore,_fakedAlert,_fakedStateCheckComparer,_fakedKeepaliveMonitor);
            Assert.NotNull(monitor);
        }

        [Fact]
        public void RunConfigurationFileRunTest()
        {
            var config = new Configuration(null);
            var monitor = new Services.Monitor.PollingMonitor(config, _fakedDataStore,_fakedAlert,_fakedStateCheckComparer,_fakedKeepaliveMonitor);
            monitor.Stop();
            Assert.NotNull(monitor);
            Assert.True(monitor.StoppedIntentionaly);
        }



    }
}
