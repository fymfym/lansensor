using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;

namespace LanSensor.PollingMonitor.Application.Services.PollingMonitor.Tools
{
    public class MonitorTools : IMonitorTools
    {
        public bool IsInsideTimeInterval(IEnumerable<TimeInterval> times, DateTime deviceLogDateValue)
        {
            var timeIntervals = times as TimeInterval[] ?? times.ToArray();

            if (!timeIntervals.Any()) return true;

            bool insideAnyGivenTime = false;

            foreach (var time in timeIntervals)
            {
                if (time.Weekdays != null && time.Weekdays.Any())
                {
                    foreach (var timeWeekday in time.Weekdays)
                    {
                        if (timeWeekday == deviceLogDateValue.DayOfWeek)
                        {
                            if (IsInsideTimeInterval(time, deviceLogDateValue))
                            {
                                insideAnyGivenTime = true;
                            }
                        }
                    }
                }
                else
                {
                    if (time.Times != null && time.Times.Any())
                    {
                        foreach (var onlyTime in time.Times)
                        {
                            if (IsInsideTimeFromTo(onlyTime, deviceLogDateValue)) return true;
                        }
                    }
                }
            }


            return insideAnyGivenTime;
        }

        private bool IsInsideTimeInterval(TimeInterval interval, DateTime deviceLogDateValue)
        {
            if (!interval.Times.Any()) return true;

            foreach (var time in interval.Times)
            {
                if (IsInsideTimeFromTo(time, deviceLogDateValue)) return true;
            }

            return true;
        }

        private bool IsInsideTimeFromTo(TimeFromTo timeFromTo, DateTime deviceLogDateValue)
        {
            return timeFromTo.From.GetNumber() < GetTimeNumber(deviceLogDateValue) &&
                   timeFromTo.To.GetNumber() > GetTimeNumber(deviceLogDateValue);
        }

        private long GetTimeNumber(DateTime time)
        {
            return time.Hour * 60 + time.Minute;
        }
    }
}
