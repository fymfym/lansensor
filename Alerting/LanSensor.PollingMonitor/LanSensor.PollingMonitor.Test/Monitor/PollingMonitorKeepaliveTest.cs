using System;
using FakeItEasy;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;
using LanSensor.Models.DeviceState;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor;
using LanSensor.PollingMonitor.Services.Monitor.DataValueToOld;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;
using LanSensor.Repository.DeviceState;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class PollingMonitorKeepaliveTest
    {
        private readonly IStateChangeMonitor _fakedStatechange;
        private readonly IGetDateTime _fakedGetDateTime;
        private readonly IDataValueToOldMonitor _fakedDataToOldMonitor;

        public PollingMonitorKeepaliveTest()
        {
            _fakedStatechange = A.Fake<IStateChangeMonitor>();
            _fakedGetDateTime = A.Fake<IGetDateTime>();
            _fakedDataToOldMonitor = A.Fake<IDataValueToOldMonitor>();
        }

        [Fact]
        public void KeepalieAlertNotDone()
        {
            var testTime = new DateTime(2019, 1, 1, 0, 0, 0);

            var keepaliveDateTime = A.Fake<IGetDateTime>();
            var deviceStateRepository = A.Fake<IDeviceStateRepository>();

            A.CallTo(() => deviceStateRepository.GetLatestDeviceStateEntity(A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceStateEntity()
                {
                    LastExecutedKeepaliveCheckDate = testTime,
                    LastKeepAliveAlert = testTime.AddDays(-2),
                    LastKnownKeepAlive = testTime.AddDays(-1)
                }
                );

            var nowDateTime = A.Fake<IGetDateTime>();
            A.CallTo(() => keepaliveDateTime.Now).Returns(testTime);
            A.CallTo(() => nowDateTime.Now).Returns(testTime.AddMinutes(61));

            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config.ApplicationConfiguration).Returns(
                new ApplicationConfiguration()
                {
                    DeviceMonitors = new[]
                    {
                        new DeviceMonitor()
                        {
                            DeviceGroupId = "dg",
                            DeviceId = "di",
                            Keepalive = new Keepalive()
                            {
                                KeepaliveDataType = "keepalive",
                                MaxMinutesSinceKeepalive = 60,
                                NotifyOnceOnly = true
                            }
                        }
                    }
                });

            var deviceLogRepository = A.Fake<IDeviceLogRepository>();
            A.CallTo(() => deviceLogRepository.GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceLogEntity()
                {
                    DataType = "sometype",
                    DeviceGroupId = "dg",
                    DeviceId = "di",
                    DateTime = testTime
                }
            );

            A.CallTo(() => deviceLogRepository.GetLatestKeepalive(A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceLogEntity()
                {
                    DataType = "keepalive",
                    DeviceGroupId = "dg",
                    DeviceId = "di",
                    DateTime = testTime
                }
                );

            var alert = A.Fake<IAlert>();

            var stateCheck = A.Fake<TimeIntervalComparer>();
            var keepalive = new Services.Monitor.Keepalive.KeepaliveMonitor(deviceLogRepository, nowDateTime);

            IPollingMonitor pollingMonitor = new Services.Monitor.PollingMonitor(config, deviceLogRepository, alert, stateCheck, keepalive, 
                _fakedStatechange,deviceStateRepository, _fakedGetDateTime, _fakedDataToOldMonitor);
            pollingMonitor.Run();
            pollingMonitor.Stop();

            A.CallTo(() => alert.SendKeepaliveMissingAlert(A<DeviceMonitor>.Ignored)).MustHaveHappened();

            Assert.NotNull(pollingMonitor);
        }

        [Fact]
        public void KeepalieAlertDone()
        {
            var testTime = new DateTime(2019, 1, 1, 0, 0, 0);

            var keepaliveDateTime = A.Fake<IGetDateTime>();
            var deviceStateRepository = A.Fake<IDeviceStateRepository>();

            A.CallTo(() => deviceStateRepository.GetLatestDeviceStateEntity(A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceStateEntity()
                {
                    LastExecutedKeepaliveCheckDate = testTime,
                    LastKeepAliveAlert = testTime.AddHours(-1),
                    LastKnownKeepAlive = testTime.AddHours(-2)
                }
                );

            var nowDateTime = A.Fake<IGetDateTime>();
            A.CallTo(() => keepaliveDateTime.Now).Returns(testTime);
            A.CallTo(() => nowDateTime.Now).Returns(testTime.AddMinutes(61));

            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config.ApplicationConfiguration).Returns(
                new ApplicationConfiguration()
                {
                    DeviceMonitors = new[]
                    {
                        new DeviceMonitor()
                        {
                            DeviceGroupId = "dg",
                            DeviceId = "di",
                            Keepalive = new Keepalive()
                            {
                                KeepaliveDataType = "keepalive",
                                MaxMinutesSinceKeepalive = 60,
                                NotifyOnceOnly = true
                            }
                        }
                    }
                });

            var deviceLogRepository = A.Fake<IDeviceLogRepository>();
            A.CallTo(() => deviceLogRepository.GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceLogEntity()
                {
                    DataType = "sometype",
                    DeviceGroupId = "dg",
                    DeviceId = "di",
                    DateTime = testTime
                }
            );

            A.CallTo(() => deviceLogRepository.GetLatestKeepalive(A<string>.Ignored, A<string>.Ignored)).Returns(
                new DeviceLogEntity()
                {
                    DataType = "keepalive",
                    DeviceGroupId = "dg",
                    DeviceId = "di",
                    DateTime = testTime
                }
                );

            var alert = A.Fake<IAlert>();

            var timeIntervalComparer = A.Fake<TimeIntervalComparer>();
            var keepalive = new Services.Monitor.Keepalive.KeepaliveMonitor(deviceLogRepository, nowDateTime);

            IPollingMonitor pollingMonitor = new Services.Monitor.PollingMonitor(
                config, deviceLogRepository, 
                alert, timeIntervalComparer, keepalive, 
                _fakedStatechange, deviceStateRepository, 
                _fakedGetDateTime, _fakedDataToOldMonitor);
            pollingMonitor.Run();
            pollingMonitor.Stop();

            A.CallTo(() => alert.SendKeepaliveMissingAlert(A<DeviceMonitor>.Ignored)).MustNotHaveHappened();

            Assert.NotNull(pollingMonitor);
        }


    }
}
