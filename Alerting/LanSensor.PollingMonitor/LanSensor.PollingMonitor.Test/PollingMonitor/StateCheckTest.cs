using System;
using System.Collections.Generic;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using Xunit;

namespace LanSensor.PollingMonitor.Test.PollingMonitor
{
    public class StateCheckTest
    {
        [Fact]
        public void RunWeekdayIntervalsTest()
        {
            var deviceLog = new DeviceLogEntity
            {
                DateTime = new DateTime(2019, 1, 1, 12, 0, 0),
                DataType = "keepalive",
                DataValue = ""
            };

            var timeIntervals = new List<TimeInterval>
            {
                new TimeInterval
                {
                    Weekdays = new[] {DayOfWeek.Thursday},
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

            ITimeIntervalMonitor checker = new TimeIntervalComparer();
            checker.GetFailedTimerInterval(timeIntervals, deviceLog);
        }
    }
}
