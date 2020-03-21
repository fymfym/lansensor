using System;
using System.Collections.Generic;
using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.CalculateAverage;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Application.Test.Services
{
    public class CalculateAverageOverHoursTest
    {
        private readonly IDeviceLogService _fakedDeviceLogService;
        private readonly IAlertService _fakedAlertService;
        private readonly IDateTimeService _fakedDateTimeService;
        private readonly DateTime _testDateTime;

        public CalculateAverageOverHoursTest()
        {
            _fakedDeviceLogService = A.Fake<IDeviceLogService>();
            _fakedAlertService = A.Fake<IAlertService>();
            _fakedDateTimeService = A.Fake<IDateTimeService>();

            _testDateTime = new DateTime(2019, 1, 1, 0, 0, 0);

            A.CallTo(() => _fakedDateTimeService.Now).Returns(_testDateTime);
        }

        [Fact]
        public void CalculateAverageOverHoursTestCanMonitorRun_MissingParameter_ReturnFalse()
        {
            var monitor = new CalculateAverageOverHours(_fakedDeviceLogService, _fakedDateTimeService, _fakedAlertService);
            var res = monitor.CanMonitorRun(new DeviceMonitor());
            Assert.False(res);
        }

        [Fact]
        public void CalculateAverageOverHoursTestCanMonitorRun_ValidParameter_ReturnTrue()
        {
            var monitor = new CalculateAverageOverHours(_fakedDeviceLogService, _fakedDateTimeService, _fakedAlertService);
            var res = monitor.CanMonitorRun(BuildDeviceMonitor());
            Assert.True(res);
        }

        [Fact]
        public void CalculateAverageOverHoursTestRun_Calculate_CallBelowAlert()
        {
            A.CallTo(() => _fakedDeviceLogService.GetPresenceListSince(A<string>.Ignored, A<string>.Ignored,
                A<string>.Ignored, A<DateTime>.Ignored)).Returns(BuildDeviceLogEntityList());

            var monitor = new CalculateAverageOverHours(_fakedDeviceLogService, _fakedDateTimeService, _fakedAlertService);
            monitor.Run(new DeviceStateEntity(), BuildDeviceMonitor(12, null));

            A.CallTo(() => _fakedAlertService.SendTextMessage(A<DeviceMonitor>.Ignored, A<string>.Ignored))
                .MustHaveHappened();
        }

        [Fact]
        public void CalculateAverageOverHoursTestRun_Calculate_CallAboveAlert()
        {
            A.CallTo(() => _fakedDeviceLogService.GetPresenceListSince(A<string>.Ignored, A<string>.Ignored,
                A<string>.Ignored, A<DateTime>.Ignored)).Returns(BuildDeviceLogEntityList());

            var monitor = new CalculateAverageOverHours(_fakedDeviceLogService, _fakedDateTimeService, _fakedAlertService);
            monitor.Run(new DeviceStateEntity(), BuildDeviceMonitor(null, 5));

            A.CallTo(() => _fakedAlertService.SendTextMessage(A<DeviceMonitor>.Ignored, A<string>.Ignored))
                .MustHaveHappened();
        }


        [Fact]
        public void CalculateAverageOverHoursTestRun_Calculate_NoAlert()
        {
            A.CallTo(() => _fakedDeviceLogService.GetPresenceListSince(A<string>.Ignored, A<string>.Ignored,
                A<string>.Ignored, A<DateTime>.Ignored)).Returns(BuildDeviceLogEntityList());

            var monitor = new CalculateAverageOverHours(_fakedDeviceLogService, _fakedDateTimeService, _fakedAlertService);
            monitor.Run(new DeviceStateEntity(), BuildDeviceMonitor(1, 15));

            A.CallTo(() => _fakedAlertService.SendTextMessage(A<DeviceMonitor>.Ignored, A<string>.Ignored))
                .MustNotHaveHappened();
        }


        private static DeviceMonitor BuildDeviceMonitor(double? alertBelow = 5, double? alertAbove = 10)
        {
            return new DeviceMonitor
            {
                DeviceGroupId = "",
                DeviceId = "",
                AverageOverHour = new AverageOverHour()
                {
                    DataValue = "",
                    Hours = 12,
                    AlertBelow = alertBelow,
                    AlertAbove = alertAbove
                }
            };
        }

        private IEnumerable<DeviceLogEntity> BuildDeviceLogEntityList()
        {
            return new List<DeviceLogEntity>
            {
                new DeviceLogEntity()
                {
                    DateTime = _testDateTime.AddHours(-1),
                    DataValue = "6"
                },
                new DeviceLogEntity()
                {
                    DateTime = _testDateTime.AddHours(-1),
                    DataValue = "15"
                }
            };
        }
    }
}
