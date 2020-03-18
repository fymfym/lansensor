using System.Collections.Generic;
using System.Linq;
using LanSensor.PollingMonitor.Application.Services.PollingMonitor.Monitors.TimeInterval;
using LanSensor.PollingMonitor.Domain.Models;

namespace LanSensor.PollingMonitor.Services.Monitor.TimeInterval
{
    public class TimeIntervalComparer : ITimeIntervalMonitor
    {
        public Domain.Models.TimeInterval GetFailedTimerInterval(
            IEnumerable<Domain.Models.TimeInterval> timeIntervals,
            DeviceLogEntity presenceRecord)
        {
            if (timeIntervals == null) return null;
            if (presenceRecord == null) return timeIntervals.FirstOrDefault();

            foreach (var interval in timeIntervals)
            {
                var weekDays = PresenceInConfiguredWeekdays(interval, presenceRecord);
                if (!weekDays) return null;

                var times = PresenceTimeInConfiguredTimes(interval, presenceRecord);
                if (!times) return null;

                if (FormatString(interval?.DataValue) != FormatString(presenceRecord.DataValue)) return interval;
            }

            return null;
        }

        private static string FormatString(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.ToLower().Trim();
        }

        private static bool PresenceInConfiguredWeekdays(
            Domain.Models.TimeInterval timeInterval,
            DeviceLogEntity presenceRecord)
        {
            if (timeInterval?.Weekdays == null) return true;
            return !timeInterval.Weekdays.Any() || timeInterval.Weekdays.Contains(presenceRecord.DateTime.DayOfWeek);
        }

        private static bool PresenceTimeInConfiguredTimes(
            Domain.Models.TimeInterval timeInterval,
            DeviceLogEntity presenceRecord)
        {
            if (timeInterval?.Times == null) return true;
            if (!timeInterval.Times.Any()) return true;

            var res = timeInterval.Times.Any(x =>
                GetTimeNumber(presenceRecord.DateTime.Hour, presenceRecord.DateTime.Minute) >= x.From.GetNumber() &&
                GetTimeNumber(presenceRecord.DateTime.Hour, presenceRecord.DateTime.Minute) <= x.To.GetNumber());
            return res;
        }

        private static long GetTimeNumber(int hour, int minute)
        {
            return hour * 60 + minute;
        }
    }
}
