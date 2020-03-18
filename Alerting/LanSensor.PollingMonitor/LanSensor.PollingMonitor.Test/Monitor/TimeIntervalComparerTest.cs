using System;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Services.Monitor.TimeInterval;
using Xunit;

namespace LanSensor.PollingMonitor.Test.Monitor
{
    public class TimeIntervalComparerTest
    {
        private readonly DateTime _testDateTime = new DateTime(2019, 1, 1, 12, 30, 00); // Tuesday
        private const string DataType = "closed";

        [Fact]
        public void GetFailedTimerIntervalWithEmptyDataReturnsNull()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new TimeInterval[]
            {
            };

            var log = new DeviceLogEntity
            {
                DateTime = _testDateTime,
                DataValue = DataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }

        [Fact]
        public void GetFailedTimerInterval_WrongDayOfWeek_ValueWeekReturnsNoTimeInterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval
                {
                    DataValue = DataType,
                    Weekdays = new[] {DayOfWeek.Monday}
                }
            };

            var log = new DeviceLogEntity
            {
                DateTime = _testDateTime,
                DataValue = "other"
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }

        [Fact]
        public void GetFailedTimerIntervalRightDataValueWrongDayOfWeekReturnsNoTimeInterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval
                {
                    DataValue = DataType,
                    Weekdays = new[] {DayOfWeek.Monday}
                }
            };
            var log = new DeviceLogEntity
            {
                DateTime = _testDateTime,
                DataValue = DataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }


        [Fact]
        public void GetFailedTimerIntervalOtherDataValueWeekReturnsNull()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval
                {
                    DataValue = DataType,
                    Weekdays = new[] {DayOfWeek.Monday}
                }
            };
            var log = new DeviceLogEntity
            {
                DateTime = _testDateTime,
                DataValue = "Other"
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }

        [Fact]
        public void GetFailedTimeIntervalReturnsInterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval
                {
                    Weekdays = new[] {DayOfWeek.Tuesday}
                }
            };
            var log = new DeviceLogEntity
            {
                DateTime = _testDateTime,
                DataValue = DataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetFailedTimeInterval_WithoutWeekday_ReturnsInterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval
                {
                    DataValue = "Test"
                }
            };

            var log = new DeviceLogEntity
            {
                DateTime = _testDateTime,
                DataValue = DataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetFailedTimerIntervalInsideSpanRightValueReturnsTimeInterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval
                {
                    DataValue = DataType,
                    Times = new[]
                    {
                        new TimeFromTo {
                            From = new SingleTime
                            {
                                Hour = _testDateTime.Hour - 1,
                                Minute = 0
                            },
                            To = new SingleTime
                            {
                                Hour = _testDateTime.Hour + 1,
                                Minute = 0
                            }
                        }
                    }
                }
            };

            var log = new DeviceLogEntity
            {
                DateTime = _testDateTime,
                DataValue = DataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }

        [Fact]
        public void GetFailedTimerIntervalInsideSpanWrongValueReturnsNull()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval
                {
                    DataValue = DataType,
                    Weekdays = new[]
                    {
                        DayOfWeek.Monday
                    },
                    Times = new[]
                    {
                        new TimeFromTo {
                            From = new SingleTime
                            {
                                Hour = _testDateTime.Hour - 1,
                                Minute = 0
                            },
                            To = new SingleTime
                            {
                                Hour = _testDateTime.Hour + 1,
                                Minute = 0
                            }
                        }
                    }
                }
            };
            var log = new DeviceLogEntity
            {
                DateTime = _testDateTime
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }

        [Fact]
        public void GetFailedTimerIntervalOutsideSpanReturnsTimeInterval()
        {
            ITimeIntervalMonitor interval = new TimeIntervalComparer();

            var intervals = new[]
            {
                new TimeInterval
                {
                    DataValue = DataType,
                    Times = new[]
                    {
                        new TimeFromTo {
                            From = new SingleTime
                            {
                                Hour = _testDateTime.Hour - 2,
                                Minute = 0
                            },
                            To = new SingleTime
                            {
                                Hour = _testDateTime.Hour - 1,
                                Minute = 0
                            }
                        }
                    }
                }
            };

            var log = new DeviceLogEntity
            {
                DateTime = _testDateTime,
                DataValue = DataType
            };

            var result = interval.GetFailedTimerInterval(intervals, log);
            Assert.Null(result);
        }
    }
}
