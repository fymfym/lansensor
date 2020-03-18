﻿using System;
using System.Threading.Tasks;
using FakeItEasy;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor.KeepAlive;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class KeepAliveMonitorTest
    {
        private readonly IDeviceLogService _fakedService;

        public KeepAliveMonitorTest()
        {
            _fakedService = A.Fake<IDeviceLogService>();
            A.CallTo(() => _fakedService.GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                .Returns(Task.FromResult(new DeviceLogEntity
                {
                    DateTime = new DateTime(1, 1, 1, 1, 1, 10)
                }));
        }

        [Fact]
        public void KeepAliveMonitorCanMonitorRun_MonitorNoDeviceGroupId_ReturnsFalse()
        {
            var repository = A.Fake<IDeviceLogService>();
            var getDateTime = A.Fake<IDateTimeService>();
            var alert = A.Fake<IAlert>();

            var keepAliveMonitor = new KeepAliveMonitor(repository, getDateTime, alert);

            var result = keepAliveMonitor.CanMonitorRun(
                new DeviceMonitor
                {
                    KeepAlive = new KeepAlive
                    {
                        KeepAliveDataType = "keepalive",
                        MaxMinutesSinceKeepAlive = 1
                    }
                });

            Assert.False(result);
        }

        [Fact]
        public void KeepAliveMonitorCanMonitorRun_MonitorNoDeviceId_ReturnsFalse()
        {
            var repository = A.Fake<IDeviceLogService>();
            var getDateTime = A.Fake<IDateTimeService>();
            var alert = A.Fake<IAlert>();

            var keepAliveMonitor = new KeepAliveMonitor(repository, getDateTime, alert);

            var result = keepAliveMonitor.CanMonitorRun(
                new DeviceMonitor
                {
                    DeviceGroupId = "",
                    KeepAlive = new KeepAlive
                    {
                        KeepAliveDataType = "keepalive",
                        MaxMinutesSinceKeepAlive = 1
                    }
                });

            Assert.False(result);
        }


        [Fact]
        public void KeepAliveMonitorCanMonitorRun_MonitorNoKeepAliveObject_ReturnsFalse()
        {
            var repository = A.Fake<IDeviceLogService>();
            var getDateTime = A.Fake<IDateTimeService>();
            var alert = A.Fake<IAlert>();

            var keepAliveMonitor = new KeepAliveMonitor(repository, getDateTime, alert);

            var result = keepAliveMonitor.CanMonitorRun(
                new DeviceMonitor
                {
                    DeviceGroupId = "",
                    DeviceId = ""
                });

            Assert.False(result);
        }

        [Fact]
        public void KeepAliveMonitor_MonitorRunWithNotOldData_MustNotCallAlert()
        {
            var getDateTime = A.Fake<IDateTimeService>();
            var alert = A.Fake<IAlert>();
            A.CallTo(() => getDateTime.Now).Returns(new DateTime(1, 1, 1, 1, 20, 1));

            var keepAliveMonitor = new KeepAliveMonitor(_fakedService, getDateTime, alert);

            A.CallTo(() => _fakedService.GetLatestKeepAlive(
                A<string>.Ignored,
                A<string>.Ignored)).Returns(new DeviceLogEntity());

            var result = keepAliveMonitor.Run(
                new DeviceStateEntity
                {
                    LastKeepAliveAlert = new DateTime(1, 1, 1, 1, 15, 0)
                },
                GetDeviceMonitor());

            A.CallTo(() => alert.SendKeepAliveMissingAlert(A<DeviceMonitor>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void KeepAliveMonitor_MonitorRunWithOldData_MustCallAlert()
        {
            var fakedTimeService = A.Fake<IDateTimeService>();
            var alert = A.Fake<IAlert>();
            A.CallTo(() => fakedTimeService.Now).Returns(new DateTime(1, 1, 2, 1, 1, 1));

            var keepAliveMonitor = new KeepAliveMonitor(_fakedService, fakedTimeService, alert);

            var result = keepAliveMonitor.Run(new DeviceStateEntity(), GetDeviceMonitor());

            A.CallTo(() => alert.SendKeepAliveMissingAlert(A<DeviceMonitor>.Ignored)).MustHaveHappened();
        }

        private static DeviceMonitor GetDeviceMonitor()
        {
            return new DeviceMonitor
            {
                DeviceGroupId = "",
                DeviceId = "",
                KeepAlive = new KeepAlive
                {
                    KeepAliveDataType = "",
                    MaxMinutesSinceKeepAlive = 60
                }
            };
        }
    }
}
