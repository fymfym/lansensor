using System;
using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.DataValueToOld;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Test.PollingMonitor
{
    public class DataValueToOldMonitorTest
    {
        private readonly IDeviceLogService _fakedDeviceLogService;
        private readonly IAlertService _fakedAlertService;
        private readonly IDateTimeService _fakedDateTimeService;

        public DataValueToOldMonitorTest()
        {
            _fakedDeviceLogService = A.Fake<IDeviceLogService>();
            _fakedAlertService = A.Fake<IAlertService>();
            _fakedDateTimeService = A.Fake<IDateTimeService>();

            A.CallTo(() => _fakedDateTimeService.Now).Returns(DateTime.MinValue.AddMinutes(1));
        }

        [Fact]
        public void CanMonitorRun_ValidRunParameters_ReturnsTrue()
        {
            var service = new DataValueToOldMonitor(_fakedDeviceLogService, _fakedAlertService, _fakedDateTimeService);

            Assert.True(service.CanMonitorRun(BuildValidDeviceMonitor()));
        }

        [Fact]
        public void CanMonitorRun_NullParameters_ReturnsFalse()
        {
            var service = new DataValueToOldMonitor(_fakedDeviceLogService, _fakedAlertService, _fakedDateTimeService);
            Assert.False(service.CanMonitorRun(null));
        }


        [Fact]
        public void Run_NoExistingData_ReturnSameState()
        {
            var service = new DataValueToOldMonitor(_fakedDeviceLogService, _fakedAlertService, _fakedDateTimeService);

            var state = new DeviceStateEntity();
            var monitor = BuildValidDeviceMonitor();

            A.CallTo(() => _fakedDeviceLogService.GetLatestPresence(A<string>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored)).Returns<DeviceLogEntity>(null);

            var newState = service.Run(state, monitor);

            Assert.NotNull(newState);
        }

        [Fact]
        public void Run_ExistingData_SendTimerIntervalAlertIsNotCalled()
        {
            var service = new DataValueToOldMonitor(_fakedDeviceLogService, _fakedAlertService, _fakedDateTimeService);

            var state = new DeviceStateEntity();
            var monitor = BuildValidDeviceMonitor();

            A.CallTo(() => _fakedDeviceLogService.GetLatestPresence(A<string>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored)).Returns(new DeviceLogEntity
            {
                DeviceGroupId = "",
                DataValue = "",
                DeviceId = ""
            });

            service.Run(state, monitor);

            A.CallTo(() =>
                    _fakedAlertService.SendTimerIntervalAlert(A<DeviceLogEntity>.Ignored, A<DeviceMonitor>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void Run_ExistingData_SendTimerIntervalAlertIsCalled()
        {
            var service = new DataValueToOldMonitor(_fakedDeviceLogService, _fakedAlertService, _fakedDateTimeService);

            var state = new DeviceStateEntity();
            var monitor = BuildValidDeviceMonitor();

            A.CallTo(() => _fakedDeviceLogService.GetLatestPresence(A<string>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored)).Returns(new DeviceLogEntity
            {
                DeviceGroupId = "",
                DataValue = "",
                DeviceId = "",
                DateTime = DateTime.MinValue.AddMinutes(1)
            });

            var newState = service.Run(state, monitor);

            A.CallTo(() =>
                    _fakedAlertService.SendTimerIntervalAlert(A<DeviceLogEntity>.Ignored, A<DeviceMonitor>.Ignored))
                .MustHaveHappened();

            Assert.NotNull(newState);
        }

        private static DeviceMonitor BuildValidDeviceMonitor()
        {
            return new DeviceMonitor
            {
                DeviceId = "1",
                DeviceGroupId = "2",
                DataValueToOld = new DataValueToOld
                {
                    MaxAgeInMinutes = 1,
                    DataValue = "value",
                }
            };
        }
    }
}
