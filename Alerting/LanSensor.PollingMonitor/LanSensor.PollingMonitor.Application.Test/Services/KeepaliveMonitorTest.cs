using System;
using System.Collections.Generic;
using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.KeepAlive;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Application.Test.Services
{
    public class KeepAliveMonitorTest
    {
        private readonly IDeviceLogService _fakedDeviceLogService;
        private readonly IAlertService _fakedAlertService;
        private readonly IDateTimeService _fakedDateTimeService;
        private readonly DateTime _testDateTime;
        private readonly IMonitorTools _monitorTools;

        public KeepAliveMonitorTest()
        {
            _fakedDeviceLogService = A.Fake<IDeviceLogService>();
            _fakedAlertService = A.Fake<IAlertService>();
            _fakedDateTimeService = A.Fake<IDateTimeService>();
            _monitorTools = A.Fake<IMonitorTools>();

            _testDateTime = new DateTime(2019, 1, 1, 0, 0, 0);
        }

        [Fact]
        public void KeepAliveMonitor_NulParameter_ReturnFalse()
        {
            var monitor = new KeepAliveMonitor(_fakedDeviceLogService, _fakedDateTimeService, _fakedAlertService);

            var res = monitor.CanMonitorRun(null);

            Assert.False(res);
        }

        [Fact]
        public void KeepAliveMonitor_UseInvalidParameters_ReturnFalse()
        {
            var monitor = new KeepAliveMonitor(_fakedDeviceLogService, _fakedDateTimeService, _fakedAlertService);

            var res = monitor.CanMonitorRun(new DeviceMonitor());

            Assert.False(res);
        }

        [Fact]
        public void KeepAliveMonitor_UseValidParameters_ReturnTrue()
        {
            var monitor = new KeepAliveMonitor(_fakedDeviceLogService, _fakedDateTimeService, _fakedAlertService);

            var res = monitor.CanMonitorRun(BuildDeviceMonitor());

            Assert.True(res);
        }


        [Fact]
        public void KeepAliveMonitor_IntervalInsideParameter_NoAlert()
        {
            A.CallTo(
                    () => _monitorTools.IsInsideTimeInterval(A<IEnumerable<TimeInterval>>.Ignored, A<DateTime>.Ignored))
                .Returns(true);

            A.CallTo(() => _fakedDeviceLogService
                    .GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(new DeviceLogEntity
                {
                    DateTime = _testDateTime
                });

            A.CallTo(() => _fakedDateTimeService.Now).Returns(_testDateTime.AddMinutes(1));

            var monitor = new KeepAliveMonitor(_fakedDeviceLogService, _fakedDateTimeService, _fakedAlertService);

            monitor.Run(new DeviceStateEntity(), BuildDeviceMonitor());

            A.CallTo(() => _fakedAlertService
                .SendTimerIntervalAlert(A<DeviceLogEntity>.Ignored, A<DeviceMonitor>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void KeepAliveMonitor_IntervalInsideParameter_MakeAlert()
        {
            A.CallTo(() => _monitorTools
                .IsInsideTimeInterval(A<IEnumerable<TimeInterval>>.Ignored, A<DateTime>.Ignored))
                .Returns(true);

            A.CallTo(() => _fakedDateTimeService.Now).Returns(_testDateTime);

            A.CallTo(() => _fakedDeviceLogService
                    .GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(
                    new DeviceLogEntity
                    {
                        DateTime = _testDateTime.AddDays(-1)
                    });

            var monitor = new KeepAliveMonitor(_fakedDeviceLogService, _fakedDateTimeService, _fakedAlertService);

            var deviceAlert = BuildDeviceMonitor();
            deviceAlert.KeepAlive.MaxMinutesSinceKeepAlive = 1;

            monitor.Run(new DeviceStateEntity(), deviceAlert);

            A.CallTo(() => _fakedAlertService.
                SendKeepAliveMissingAlert(A<DeviceMonitor>.Ignored)).MustHaveHappened();
        }

        private static DeviceMonitor BuildDeviceMonitor()
        {
            return new DeviceMonitor
            {
                DeviceGroupId = "",
                DeviceId = "",
                KeepAlive = new KeepAlive
                {
                    MaxMinutesSinceKeepAlive = 1,
                    KeepAliveDataType = "",
                    NotifyOnceOnly = false
                }
            };
        }
    }
}
