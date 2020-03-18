using System;
using System.Collections.Generic;
using System.Text;
using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.StateChange;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Application.Test.Services
{
    public class StateChangeMonitorTest
    {
        [Fact]
        public void MonitorCanRun_WithInvalidParameter_ReturnFalse()
        {
            var fakedDeviceLogService = A.Fake<IDeviceLogService>();
            var fakedAlertService = A.Fake<IAlertService>();
            var fakedMonitorTools = A.Fake<IMonitorTools>();

            var monitor = new StateChangeMonitor(
                fakedDeviceLogService,
                fakedAlertService,
                fakedMonitorTools);
        }
    }
}
