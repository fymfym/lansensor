using System;
using System.Threading.Tasks;
using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.KeepAlive;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class MonitorAliveMessageMonitorTest
    {
        private readonly IAlertService _fakedAlertService;

        public MonitorAliveMessageMonitorTest()
        {
            var fakedService = A.Fake<IDeviceLogService>();
            _fakedAlertService = A.Fake<IAlertService>();

            A.CallTo(() => fakedService.GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(Task.FromResult(new DeviceLogEntity
                {
                    DateTime = new DateTime(1, 1, 1, 1, 1, 10)
                }));
        }

        [Fact]
        public void MonitorAliveMessageMonitorCanMonitorRun_MonitorNoObject_ReturnsFalse()
        {
            var repository = A.Fake<IDeviceLogService>();
            var getDateTime = A.Fake<IDateTimeService>();

            var keepAliveMonitor = new KeepAliveMonitor(repository, getDateTime, _fakedAlertService);

            var result = keepAliveMonitor.CanMonitorRun(
                new DeviceMonitor());

            Assert.False(result);
        }

        [Fact]
        public void MonitorAliveMessageMonitorCanMonitorRun_Null_ReturnsFalse()
        {
            var repository = A.Fake<IDeviceLogService>();
            var getDateTime = A.Fake<IDateTimeService>();

            var keepAliveMonitor = new KeepAliveMonitor(repository, getDateTime, _fakedAlertService);

            var result = keepAliveMonitor.CanMonitorRun(null);

            Assert.False(result);
        }
    }
}
