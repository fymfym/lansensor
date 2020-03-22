using System;
using System.Collections.Generic;
using System.Linq;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Application.Services.PollingMonitor.Tools
{
    public class MonitorTools : IMonitorTools
    {
        public bool IsInsideTimeInterval(IEnumerable<TimeInterval> times, DateTime deviceLogDateValue)
        {
            if (times == null) return true;

            var timeIntervals = times as TimeInterval[] ?? times.ToArray();

            if (!timeIntervals.Any()) return true;

            var insideAnyGivenTime = false;

            foreach (var time in timeIntervals)
            {
                if (time.Weekdays != null && time.Weekdays.Any())
                {
                    foreach (var timeWeekday in time.Weekdays)
                    {
                        if (timeWeekday != deviceLogDateValue.DayOfWeek) continue;

                        if (IsInsideTimeInterval(time, deviceLogDateValue))
                        {
                            insideAnyGivenTime = true;
                        }
                    }
                }
                else
                {
                    if (time.Times == null || !time.Times.Any()) continue;

                    if (time.Times.Any(onlyTime => IsInsideTimeFromTo(onlyTime, deviceLogDateValue)))
                    {
                        return true;
                    }
                }
            }


            return insideAnyGivenTime;
        }

        public MonitorState GetMonitorState(DeviceStateEntity deviceStateEntity, DeviceMonitor monitor)
        {
            if (deviceStateEntity.MonitorStates == null) return new MonitorState
            {
                MonitorName = monitor.Name
            };

            return deviceStateEntity.MonitorStates.FirstOrDefault(x => x.MonitorName == monitor.Name);
        }

        public void SetMonitorState(DeviceStateEntity deviceStateEntity, DeviceMonitor monitor, MonitorState monitorState)
        {
            if (deviceStateEntity.MonitorStates == null) deviceStateEntity.MonitorStates = new List<MonitorState>();

            var state = deviceStateEntity.MonitorStates.FirstOrDefault(x => x.MonitorName == monitor.Name);
            monitorState.MonitorName = monitor.Name;

            if (state == null)
            {
                deviceStateEntity.MonitorStates.Add(monitorState);
            }
            else
            {
                deviceStateEntity.MonitorStates[deviceStateEntity.MonitorStates.IndexOf(state)] = monitorState;
            }
        }

        private static bool IsInsideTimeInterval(TimeInterval interval, DateTime deviceLogDateValue)
        {
            return !interval.Times.Any() || interval.Times.Any(time => IsInsideTimeFromTo(time, deviceLogDateValue));
        }

        private static bool IsInsideTimeFromTo(TimeFromTo timeFromTo, DateTime deviceLogDateValue)
        {
            return timeFromTo.From.GetNumber() < GetTimeNumber(deviceLogDateValue) &&
                   timeFromTo.To.GetNumber() > GetTimeNumber(deviceLogDateValue);
        }

        private static long GetTimeNumber(DateTime time)
        {
            return time.Hour * 60 + time.Minute;
        }
    }
}
