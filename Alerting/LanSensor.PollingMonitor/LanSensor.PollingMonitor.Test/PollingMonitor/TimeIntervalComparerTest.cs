using System;
using System.Collections.Generic;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using Xunit;

namespace LanSensor.PollingMonitor.Test.PollingMonitor
{
    public class TimeIntervalComparerTest
    {
        [Fact]
        public void RunWeekdayIntervalsTest_OutsideNoDataValue_NoFailedInterval()
        {
            var deviceLog = new DeviceLogEntity
            {
                DateTime = new DateTime(2019, 1, 1, 12, 0, 0),
                DataType = "keepalive",
                DataValue = ""
            };

            var timeIntervals = BuildTimeIntervals();

            ITimeIntervalMonitor checker = new TimeIntervalComparer();
            var result = checker.GetFailedTimerInterval(timeIntervals, deviceLog);
            Assert.Null(result);
        }

        [Fact]
        public void RunWeekdayIntervalsTest_OutsideDataValue_NoFailedInterval()
        {
            var deviceLog = new DeviceLogEntity
            {
                DateTime = new DateTime(2019, 1, 1, 12, 0, 0),
                DataType = "keepalive",
                DataValue = "someValue"
            };

            var timeIntervals = BuildTimeIntervals();

            ITimeIntervalMonitor checker = new TimeIntervalComparer();
            var result = checker.GetFailedTimerInterval(timeIntervals, deviceLog);
            Assert.Null(result);
        }

        [Fact]
        public void RunWeekdayIntervalsTest_InsideNoDataValue_FailedInterval()
        {
            var deviceLog = new DeviceLogEntity
            {
                DateTime = new DateTime(2019, 1, 3, 12, 0, 0),
                DataType = "keepalive",
                DataValue = ""
            };

            var timeIntervals = BuildTimeIntervals();

            ITimeIntervalMonitor checker = new TimeIntervalComparer();
            var result = checker.GetFailedTimerInterval(timeIntervals, deviceLog);
            Assert.Null(result);
        }

        [Fact]
        public void RunWeekdayIntervalsTest_Inside_ReturnsOneInterval()
        {
            var deviceLog = new DeviceLogEntity
            {
                DateTime = new DateTime(2019, 1, 3, 12, 0, 0),
                DataType = "keepalive",
                DataValue = "someValue"
            };

            var timeIntervals = BuildTimeIntervals("someValue");

            ITimeIntervalMonitor checker = new TimeIntervalComparer();
            var result = checker.GetFailedTimerInterval(timeIntervals, deviceLog);
            Assert.Null(result);
        }

        [Fact]
        public void RunWeekdayIntervalsTest_InsideWrongValue_ReturnsOneInterval()
        {
            var deviceLog = new DeviceLogEntity
            {
                DateTime = new DateTime(2019, 1, 3, 12, 0, 0),
                DataType = "keepalive",
                DataValue = "someValue"
            };

            var timeIntervals = BuildTimeIntervals("someOtherValue");

            ITimeIntervalMonitor checker = new TimeIntervalComparer();
            var result = checker.GetFailedTimerInterval(timeIntervals, deviceLog);
            Assert.NotNull(result);
        }

        private static IEnumerable<TimeInterval> BuildTimeIntervals(string dataValue = null)
        {
            return new List<TimeInterval>
            {
                new TimeInterval
                {
                    Weekdays = new[] {DayOfWeek.Thursday},
                    DataValue = dataValue,
                    Times = new List<TimeFromTo>
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
                    }
                }
            };
        }
    }
}
