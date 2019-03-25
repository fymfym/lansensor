using System;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;
using LanSensor.PollingMonitor.Services.Monitor.DataValueToOld;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class DataValueToOldMonitorTest
    {
        private readonly IDataValueToOldMonitor _monitor;
        private readonly DataValueToOld _dataValueToOld;

        public DataValueToOldMonitorTest()
        {
            _monitor = new DataValueToOldMonitor();
            _dataValueToOld = new DataValueToOld()
            {
                MaxAgeInMinutes = 120
            };
        }

        [Fact]
        public void IsDataValueToOldInRangeTest()
        {
            var deviceLog = new DeviceLogEntity()
            {
                DateTime = DateTime.Now
            };

            var result = _monitor.IsDataValueToOld(deviceLog, _dataValueToOld);
            Assert.True(result);
        }

        [Fact]
        public void IsDataValueToOldBeforeRangeTest()
        {
            var deviceLog = new DeviceLogEntity()
            {
                DateTime = DateTime.Now.AddMinutes(-130)
            };

            var result = _monitor.IsDataValueToOld(deviceLog, _dataValueToOld);
            Assert.False(result);
        }

        [Fact]
        public void IsDataValueToNewBeforeRangeTest()
        {
            var deviceLog = new DeviceLogEntity()
            {
                DateTime = DateTime.Now.AddMinutes(10)
            };

            var result = _monitor.IsDataValueToOld(deviceLog, _dataValueToOld);
            Assert.True(result);
        }


    }
}
