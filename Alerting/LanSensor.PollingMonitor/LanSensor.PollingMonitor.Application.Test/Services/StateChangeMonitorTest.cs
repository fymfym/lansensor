using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.StateChange;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Application.Test.Services
{
    public class StateChangeMonitorTest
    {
        private readonly IDeviceLogService _fakedDeviceLogService;
        private readonly IAlertService _fakedAlertService;

        public StateChangeMonitorTest()
        {
            _fakedDeviceLogService = A.Fake<IDeviceLogService>();
            _fakedAlertService = A.Fake<IAlertService>();
        }

        [Fact]
        public void MonitorCanRun_WithInvalidParameter_ReturnFalse()
        {
            var monitor = new StateChangeMonitor(
                _fakedDeviceLogService,
                _fakedAlertService);

            var result = monitor.CanMonitorRun(null);

            Assert.False(result);
        }

        [Fact]
        public void MonitorCanRun_WithValidToParameter_ReturnTrue()
        {
            var monitor = new StateChangeMonitor(
                _fakedDeviceLogService,
                _fakedAlertService);

            var result = monitor.CanMonitorRun(new DeviceMonitor
            {
                StateChangeNotification = new StateChangeNotification
                {
                    OnDataValueChangeTo = ""
                }
            });

            Assert.True(result);
        }

        [Fact]
        public void MonitorCanRun_WithValidFromParameter_ReturnTrue()
        {
            var monitor = new StateChangeMonitor(
                _fakedDeviceLogService,
                _fakedAlertService);

            var result = monitor.CanMonitorRun(new DeviceMonitor
            {
                StateChangeNotification = new StateChangeNotification
                {
                    OnDataValueChangeFrom = ""
                }
            });

            Assert.True(result);
        }

        [Fact]
        public void MonitorCanRun_WithBothValidParameter_ReturnTrue()
        {
            var monitor = new StateChangeMonitor(
                _fakedDeviceLogService,
                _fakedAlertService);

            var result = monitor.CanMonitorRun(new DeviceMonitor
            {
                StateChangeNotification = new StateChangeNotification
                {
                    OnDataValueChangeFrom = "",
                    OnDataValueChangeTo = ""
                }
            });

            Assert.True(result);
        }
    }
}
