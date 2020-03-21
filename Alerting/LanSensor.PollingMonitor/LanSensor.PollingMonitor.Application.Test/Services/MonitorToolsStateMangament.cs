using System.Collections.Generic;
using System.Linq;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Tools;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Application.Test.Services
{
    public class MonitorToolsStateManagement
    {
        private readonly IMonitorTools _tools;
        private const string MonitorName = "MonitorName";

        public MonitorToolsStateManagement()
        {
            _tools = new MonitorTools();
        }

        [Fact]
        public void State_AddNewState_StatePresent()
        {
            var state = BuildDeviceStateEntity();
            _tools.SetMonitorState(state, BuildDeviceMonitor(), BuildMonitorState());
            Assert.NotEmpty(state.MonitorStates.Where(x => x.MonitorName == MonitorName));
        }

        [Fact]
        public void State_GetStateStatePresent_ReturnsState()
        {
            var state = BuildDeviceStateEntity(BuildMonitorState("test"));
            _tools.GetMonitorState(state, BuildDeviceMonitor());
            Assert.NotEmpty(state.MonitorStates.Where(x => x.MonitorName == MonitorName && x.Value == "test"));
        }

        [Fact]
        public void State_GetStateNotPresent_ReturnsState()
        {
            var state = BuildDeviceStateEntity();
            _tools.SetMonitorState(state, BuildDeviceMonitor(), BuildMonitorState());
            Assert.NotEmpty(state.MonitorStates.Where(x => x.MonitorName == MonitorName));
        }


        [Fact]
        public void State_UpdateState_StatePresent()
        {
            var state = BuildDeviceStateEntity(BuildMonitorState());
            _tools.SetMonitorState(state, BuildDeviceMonitor(), BuildMonitorState("test2"));
            Assert.NotEmpty(state.MonitorStates.Where(x => x.MonitorName == MonitorName && x.Value == "test2"));
        }

        private DeviceStateEntity BuildDeviceStateEntity(MonitorState monitorPresent = null)
        {
            var res = new DeviceStateEntity();

            if (monitorPresent != null)
            {
                res.MonitorStates = new List<MonitorState> {monitorPresent};
            }

            return res;
        }

        private DeviceMonitor BuildDeviceMonitor()
        {
            var res = new DeviceMonitor()
            {
                Name = MonitorName
            };

            return res;
        }


        private MonitorState BuildMonitorState(string value = null)
        {
            var res = new MonitorState()
            {
                MonitorName = MonitorName
            };

            if (value != null)
            {
                res.Value = value;
            }

            return res;
        }
    }
}
