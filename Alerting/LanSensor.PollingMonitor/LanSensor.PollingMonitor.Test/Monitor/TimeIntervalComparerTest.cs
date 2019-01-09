using System;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class TimeIntervalComparerTest
    {
        private readonly DateTime _testDateTime = new DateTime(2019,1,1,12,30,00); // Tuesday
        private readonly string _dataType = "closed";

        [Fact]
        public void GetFailedTimerIntervalWithEmptyDataReturnsNull()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new TimeInterval[]
            {

            };
            var log = new DeviceLogEntity()
            {
                DateTime = _testDateTime,
                DataValue = _dataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }

        [Fact]
        public void GetFailedTimerIntervalWrongDayOfWeekValueWeekReturnsTimeinterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval()
                {
                    DataValue = _dataType,
                    Weekdays = new [] {DayOfWeek.Monday}
                }
            };
            var log = new DeviceLogEntity()
            {
                DateTime = _testDateTime,
                DataValue = "other"
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetFailedTimerIntervalRightDataValueWrongDayOfWeekReturnsNoTimeinterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval()
                {
                    DataValue = _dataType,
                    Weekdays = new [] {DayOfWeek.Monday}
                }
            };
            var log = new DeviceLogEntity()
            {
                DateTime = _testDateTime,
                DataValue = _dataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }


        [Fact]
        public void GetFailedTimerIntervalOtherDataValueWeekReturnsTimeinterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval()
                {
                    DataValue = _dataType,
                    Weekdays = new [] {DayOfWeek.Monday}
                }
            };
            var log = new DeviceLogEntity()
            {
                DateTime = _testDateTime,
                DataValue = "Other"
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetFailedTimerIntervalReturnsNull()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval()
                {
                    Weekdays = new [] {DayOfWeek.Tuesday}
                }
            };
            var log = new DeviceLogEntity()
            {
                DateTime = _testDateTime,
                DataValue = _dataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }

        [Fact]
        public void GetFailedTimerIntervalInsideSpanRightValueReturnsTimeInterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval()
                {
                    DataValue = _dataType,
                    Times = new []
                    {
                        new TimeFromTo() {
                            From = new SingleTime()
                            {
                                Hour = _testDateTime.Hour-1,
                                Minute = 0
                            },
                            To = new SingleTime()
                            {
                                Hour = _testDateTime.Hour+1,
                                Minute = 0
                            }
                        }
                    }
                }
            };
            var log = new DeviceLogEntity()
            {
                DateTime = _testDateTime,
                DataValue = _dataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }

        [Fact]
        public void GetFailedTimerIntervalInsideSpanWrongValueReturnsTimeInterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval()
                {
                    DataValue = _dataType,
                    Times = new []
                    {
                        new TimeFromTo() {
                            From = new SingleTime()
                            {
                                Hour = _testDateTime.Hour-1,
                                Minute = 0
                            },
                            To = new SingleTime()
                            {
                                Hour = _testDateTime.Hour+1,
                                Minute = 0
                            }
                        }
                    }
                }
            };
            var log = new DeviceLogEntity()
            {
                DateTime = _testDateTime,
                DataValue = "other"
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetFailedTimerIntervalOutsieSpanReturnsTimeInterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval()
                {
                    DataValue = _dataType,
                    Times = new []
                    {
                        new TimeFromTo() {
                            From = new SingleTime()
                            {
                                Hour = _testDateTime.Hour-2,
                                Minute = 0
                            },
                            To = new SingleTime()
                            {
                                Hour = _testDateTime.Hour-1,
                                Minute = 0
                            }
                        }
                    }
                }
            };
            var log = new DeviceLogEntity()
            {
                DateTime = _testDateTime,
                DataValue = _dataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }

    }
}
