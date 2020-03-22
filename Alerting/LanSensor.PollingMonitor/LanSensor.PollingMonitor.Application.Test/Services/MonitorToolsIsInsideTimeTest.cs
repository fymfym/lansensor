using System;
using System.Collections.Generic;
using System.Linq;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Tools;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Services;
using Xunit;

namespace LanSensor.PollingMonitor.Application.Test.Services
{
    public class MonitorToolsIsInsideTimeTest
    {
        private readonly IMonitorTools _tools;

        public MonitorToolsIsInsideTimeTest()
        {
            _tools = new MonitorTools();
        }

        [Fact]
        public void RunTimeIntervalTest_NoInterval_NoFailedInterval()
        {
            var dateTimeValue = new DateTime(2019, 1, 1, 12, 0, 0);

            var result = _tools.IsInsideTimeInterval(new List<TimeInterval>(), dateTimeValue);
            Assert.True(result);
        }

        [Fact]
        public void RunWeekdayIntervalsTest_DayAndTimeMatch_NoFailedInterval()
        {
            var dateTimeValue = new DateTime(2019, 1, 1, 12, 0, 0);

            var timeIntervals = BuildTimeIntervals(DayOfWeek.Tuesday, true);
            var result = _tools.IsInsideTimeInterval(timeIntervals, dateTimeValue);

            Assert.True(result);
        }

        [Fact]
        public void RunWeekdayIntervalsTest_WeekDayMatchNoTime_ReturnTrue()
        {
            var dateTimeValue = new DateTime(2019, 1, 3, 12, 0, 0);

            var timeIntervals = BuildTimeIntervals(DayOfWeek.Tuesday, false);

            var result = _tools.IsInsideTimeInterval(timeIntervals, dateTimeValue);
            Assert.False(result);
        }

        [Fact]
        public void RunWeekdayIntervalsTest_TimeMatchNoWeekDay_ReturnsTrue()
        {
            var dateTimeValue = new DateTime(2019, 1, 3, 12, 0, 0);

            var timeIntervals = new List<TimeInterval>
            {
                new TimeInterval
                {
                    Times = BuildTimeFromTo()
                }
            };

            var result = _tools.IsInsideTimeInterval(timeIntervals, dateTimeValue);
            Assert.True(result);
        }

        [Fact]
        public void RunWeekdayIntervalsTest_TimeGivenNoTimeOrDayMatch_ReturnsFalse()
        {
            var dateTimeValue = new DateTime(2019, 1, 3, 17, 0, 0);

            var timeIntervals = new List<TimeInterval>
            {
                new TimeInterval
                {
                    Times = BuildTimeFromTo()
                }
            };

            var result = _tools.IsInsideTimeInterval(timeIntervals, dateTimeValue);
            Assert.False(result);
        }

        /// <summary>
        /// holds one interval date 2019-01-01 11:00 -> 14:00 (Tuesday) if boolean is true
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<TimeInterval> BuildTimeIntervals(DayOfWeek weekDay, bool addTimes)
        {
            var res = new List<TimeInterval>
            {
                new TimeInterval
                {
                    Weekdays = new[] {weekDay}
                }
            };

            if (addTimes)
            {
                res.First().Times = BuildTimeFromTo();
            }

            return res;
        }

        private static IEnumerable<TimeFromTo> BuildTimeFromTo()
        {
            return new List<TimeFromTo>
            {
                new TimeFromTo
                {
                    From = new SingleTime
                    {
                        Hour = 11,
                        Minute = 00
                    },
                    To = new SingleTime
                    {
                        Hour = 14,
                        Minute = 00
                    }
                }
            };
        }
    }
}
