using System;
using System.Threading.Tasks;
using FakeItEasy;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor.KeepAlive;
using LanSensor.Repository.DeviceLog;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class KeepAliveMonitorTest
    {
        private readonly IDeviceLogRepository _fakedRepository;

        public KeepAliveMonitorTest()
        {
            _fakedRepository = A.Fake<IDeviceLogRepository>();
            A.CallTo(() => _fakedRepository.GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(Task.FromResult(new DeviceLogEntity
                {
                    DateTime = new DateTime(1, 1, 1, 1, 1, 10)
                }));
        }

        [Fact]
        public async Task KeepAliveMonitor_NoMonitor_ThrowsException()
        {
            var repository = A.Fake<IDeviceLogRepository>();
            var getDateTime = A.Fake<IDateTimeService>();
            var keepAliveMonitor = new KeepAliveMonitor(repository, getDateTime);

            var result = await keepAliveMonitor.IsKeepAliveWithinSpec(null);
            Assert.True(result);
        }

        [Fact]
        public async Task KeepAliveMonitor_MonitorNoDeviceGroup_ThrowsException()
        {
            var repository = A.Fake<IDeviceLogRepository>();
            var getDateTime = A.Fake<IDateTimeService>();
            var keepAliveMonitor = new KeepAliveMonitor(repository, getDateTime);

            var result = await keepAliveMonitor.IsKeepAliveWithinSpec(new DeviceMonitor
            {
                DeviceId = ""
            });

            Assert.True(result);
        }

        [Fact]
        public async Task KeepAliveMonitor_MonitorNoDeviceId_ThrowsException()
        {
            var repository = A.Fake<IDeviceLogRepository>();
            var getDateTime = A.Fake<IDateTimeService>();
            var keepAliveMonitor = new KeepAliveMonitor(repository, getDateTime);

            var result = await keepAliveMonitor.IsKeepAliveWithinSpec(new DeviceMonitor
            {
                DeviceGroupId = ""
            });

            Assert.True(result);
        }

        [Fact]
        public async Task KeepAliveMonitor_MonitorDataOutOfSpec_ReturnsFalse()
        {
            var getDateTime = A.Fake<IDateTimeService>();
            A.CallTo(() => getDateTime.Now).Returns(new DateTime(1, 1, 2, 1, 1, 1));

            var keepAliveMonitor = new KeepAliveMonitor(_fakedRepository, getDateTime);

            var result = await keepAliveMonitor.IsKeepAliveWithinSpec(GetDeviceMonitor());

            Assert.False(result);
        }

        [Fact]
        public async Task KeepAliveMonitor_MonitorDataOutOfSpec_ReturnsTrue()
        {
            var getDateTime = A.Fake<IDateTimeService>();
            A.CallTo(() => getDateTime.Now).Returns(new DateTime(1, 1, 1, 1, 1, 1));

            var keepAliveMonitor = new KeepAliveMonitor(_fakedRepository, getDateTime);

            var result = await keepAliveMonitor.IsKeepAliveWithinSpec(GetDeviceMonitor());

            Assert.True(result);
        }

        private static DeviceMonitor GetDeviceMonitor()
        {
            return new DeviceMonitor
            {
                DeviceGroupId = "",
                DeviceId = "",
                KeepAlive = new KeepAlive
                {
                    MaxMinutesSinceKeepAlive = 1
                }
            };
        }
    }
}
