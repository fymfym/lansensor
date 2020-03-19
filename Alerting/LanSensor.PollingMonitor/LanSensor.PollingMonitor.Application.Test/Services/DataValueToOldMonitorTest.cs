using System;
using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.DataValueToOld;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Application.Test.Services
{
    public class DataValueToOldMonitorTest
    {
        private readonly IDeviceLogService _fakedDeviceLogService;
        private readonly IAlertService _fakedAlertService;
        private readonly IDateTimeService _fakedDateTimeService;
        private readonly DateTime _testDateTime;

        public DataValueToOldMonitorTest()
        {
            _fakedDeviceLogService = A.Fake<IDeviceLogService>();
            _fakedAlertService = A.Fake<IAlertService>();
            _fakedDateTimeService = A.Fake<IDateTimeService>();

            _testDateTime = new DateTime(2019, 1, 1, 0, 0, 0);
        }

        [Fact]
        public void DataValueToOldCanRun_NulParameter_ReturnFalse()
        {
            var monitor = new DataValueToOldMonitor(_fakedDeviceLogService, _fakedAlertService, _fakedDateTimeService);

            var res = monitor.CanMonitorRun(null);

            Assert.False(res);
        }

        [Fact]
        public void DataValueToOldCanRun_UseInvalidParameters_ReturnFalse()
        {
            var monitor = new DataValueToOldMonitor(_fakedDeviceLogService, _fakedAlertService, _fakedDateTimeService);

            var res = monitor.CanMonitorRun(new DeviceMonitor());

            Assert.False(res);
        }

        [Fact]
        public void DataValueToOldCanRun_UseValidParameters_ReturnTrue()
        {
            var monitor = new DataValueToOldMonitor(_fakedDeviceLogService, _fakedAlertService, _fakedDateTimeService);

            var res = monitor.CanMonitorRun(BuildDeviceMonitor());

            Assert.True(res);
        }


        [Fact]
        public void DataValueToOldRun_IntervalInsideParameter_NoAlert()
        {
            var monitor = new DataValueToOldMonitor(_fakedDeviceLogService, _fakedAlertService, _fakedDateTimeService);

            A.CallTo(() => _fakedDeviceLogService
                    .GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(new DeviceLogEntity
                {
                    DateTime = _testDateTime
                });

            A.CallTo(() => _fakedDateTimeService.Now).Returns(_testDateTime.AddMinutes(1));

            var res = monitor.Run(new DeviceStateEntity(), new DeviceMonitor());

            A.CallTo(() => _fakedAlertService.SendTimerIntervalAlert(A<DeviceLogEntity>.Ignored, A<DeviceMonitor>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void DataValueToOldRun_IntervalInsideParameter_MakeAlert()
        {
            var monitor = new DataValueToOldMonitor(_fakedDeviceLogService, _fakedAlertService, _fakedDateTimeService);

            var res = monitor.Run(new DeviceStateEntity(), new DeviceMonitor());

            A.CallTo(() => _fakedAlertService.SendTimerIntervalAlert(A<DeviceLogEntity>.Ignored, A<DeviceMonitor>.Ignored)).MustHaveHappened();
        }

        private static DeviceMonitor BuildDeviceMonitor()
        {
            return new DeviceMonitor
            {
                DeviceGroupId = "",
                DeviceId = "",
                DataValueToOld = new DataValueToOld
                {
                    DataValue = "",
                    MaxAgeInMinutes = 10
                }
            };
        }
    }
}
