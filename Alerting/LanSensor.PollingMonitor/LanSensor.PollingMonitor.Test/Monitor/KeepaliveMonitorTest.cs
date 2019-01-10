using FakeItEasy;
using LanSensor.Models.Configuration;
using LanSensor.PollingMonitor.Services.Alert;
using LanSensor.PollingMonitor.Services.DateTime;
using LanSensor.PollingMonitor.Services.Monitor;
using LanSensor.PollingMonitor.Services.Monitor.StateChange;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using LanSensor.Repository.DeviceLog;
using System;
using LanSensor.Repository.DeviceState;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class KeepaliveMonitorTest
    {
        private readonly IStateChangeMonitor _fakedStatechange;
        private readonly IDeviceLogRepository _fakedDeviceLog;
        private readonly IDeviceStateRepository _fakedDeviceStage;

        public KeepaliveMonitorTest()
        {
            _fakedStatechange = A.Fake<IStateChangeMonitor>();
            _fakedDeviceLog = A.Fake<IDeviceLogRepository>();
            _fakedDeviceStage = A.Fake<IDeviceStateRepository>();
        }


        [Fact]
        public void KeepalieWithinSpec()
        {
            var testTime = new DateTime(2019, 1, 1, 0, 0, 0);

            var testDateTime = A.Fake<IGetDateTime>();
            A.CallTo(() => testDateTime.Now).Returns(testTime);

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
                                MaxMinutesSinceKeepalive = 60
                            }
                        }
                    }
                });

            var repository = A.Fake<IDeviceLogRepository>();
            A.CallTo(() => repository.GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(
                new Models.DeviceLog.DeviceLogEntity()
                {
                    DataType = "keepalive",
                    DeviceGroupId = "",
                    DeviceId = "",
                    DateTime = testTime
                }
            );

            var alert = A.Fake<IAlert>();

            var stateCheck = A.Fake<TimeIntervalComparer>();
            var keepalive = new Services.Monitor.Keepalive.KeepaliveMonitor(repository, testDateTime);

            IPollingMonitor pollingMonitor = new Services.Monitor.PollingMonitor(config, repository, alert, stateCheck, keepalive, _fakedStatechange, _fakedDeviceStage, _fakedDeviceLog);
            pollingMonitor.Run();
            pollingMonitor.Stop();

            A.CallTo(() => alert.SendKeepaliveMissingAlert(A<DeviceMonitor>.Ignored)).MustNotHaveHappened();

            Assert.NotNull(pollingMonitor);
        }

        [Fact]
        public void KeepalieOutsideSpec()
        {
            var testTime = new DateTime(2019, 1, 1, 0, 0, 0);

            var keepaliveDateTime = A.Fake<IGetDateTime>();
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
                                MaxMinutesSinceKeepalive = 60
                            }
                        }
                    }
                });

            var repository = A.Fake<IDeviceLogRepository>();
            A.CallTo(() => repository.GetLatestPresence(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(
                new Models.DeviceLog.DeviceLogEntity()
                {
                    DataType = "keepalive",
                    DeviceGroupId = "",
                    DeviceId = "",
                    DateTime = testTime
                }
            );

            var alert = A.Fake<IAlert>();

            var stateCheck = A.Fake<TimeIntervalComparer>();
            var keepalive = new Services.Monitor.Keepalive.KeepaliveMonitor(repository, nowDateTime);

            IPollingMonitor pollingMonitor = new Services.Monitor.PollingMonitor(config, repository, alert, stateCheck, keepalive, _fakedStatechange,_fakedDeviceStage, _fakedDeviceLog);
            pollingMonitor.Run();
            pollingMonitor.Stop();

            A.CallTo(() => alert.SendKeepaliveMissingAlert(A<DeviceMonitor>.Ignored)).MustHaveHappened();

            Assert.NotNull(pollingMonitor);
        }

    }
}
