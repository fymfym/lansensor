using System;
using System.Collections.Generic;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using Xunit;

namespace LanSensor.PollingMonitor.Test
{
    public class StateCheckTest
    {

        [Fact]
        public void RunWeekdayIntervalsTest()
        {

            var deviceLog = new DeviceLog()
            {
                DateTime = new DateTime(2019, 1, 1, 12, 0, 0),
                DataType = "keepalive",
                DataValue = ""
            };

            var timeIntervals = new List<TimeInterval>()
            {
                new TimeInterval()
                {
                    Weekdays = new[]{DayOfWeek.Thursday},
                    Times = new List<TimeFromTo>()
                    {
                        new TimeFromTo()
                        {
                            From = new SingleTime()
                            {
                                Hour = 11,
                                Minute = 00
                            },
                            To = new SingleTime()
                            {
                                Hour = 14,
                                Minute = 00
                            }

                        }
                    }
                }
            };

            ITimeIntervalComparer checker = new TimeIntervalComparer();
            checker.GetFailedTimerInterval(timeIntervals, deviceLog);

        }


    }
}
