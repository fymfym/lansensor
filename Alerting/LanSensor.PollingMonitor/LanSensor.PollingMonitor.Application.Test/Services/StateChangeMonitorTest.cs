using FakeItEasy;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.StateChange;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Application.Test.Services
{
    public class StateChangeMonitorTest
    {
        private IDeviceLogService _fakedDeviceLogService;
        private IAlertService _fakedAlertService;
        private IMonitorTools _fakedMonitorTools;

        public StateChangeMonitorTest()
        {
            _fakedDeviceLogService = A.Fake<IDeviceLogService>();
            _fakedAlertService = A.Fake<IAlertService>();
            _fakedMonitorTools = A.Fake<IMonitorTools>();
        }

        [Fact]
        public void MonitorCanRun_WithInvalidParameter_ReturnFalse()
        {
            var monitor = new StateChangeMonitor(
                _fakedDeviceLogService,
                _fakedAlertService,
                _fakedMonitorTools);

            var result = monitor.CanMonitorRun(null);

            Assert.False(result);
        }

        [Fact]
        public void MonitorCanRun_WithValidToParameter_ReturnTrue()
        {
            var monitor = new StateChangeMonitor(
                _fakedDeviceLogService,
                _fakedAlertService,
                _fakedMonitorTools);

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
                _fakedAlertService,
                _fakedMonitorTools);

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
                _fakedAlertService,
                _fakedMonitorTools);

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
