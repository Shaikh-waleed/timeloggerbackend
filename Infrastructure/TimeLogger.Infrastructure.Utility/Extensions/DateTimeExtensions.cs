using TimeLogger.Infrastructure.Utility.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Utility.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToStartOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime ToEndOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, DateTimeKind.Utc);
        }

        public static DateTime? ToStartOfDay(this DateTime? dateTime)
        {
            if (dateTime == null)
                return null;
            return new DateTime(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime? ToEndOfDay(this DateTime? dateTime)
        {
            if (dateTime == null)
                return null;
            return new DateTime(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day, 23, 59, 59,
                DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns TimeZone adjusted time for a given from a Utc or local time.
        /// Date is first converted to UTC then adjusted.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneTime(this DateTime time, string timeZoneId = "Pakistan Standard Time")
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return time.ToTimeZoneTime(tzi);
        }

        /// <summary>
        /// Returns TimeZone adjusted time for a given from a Utc or local time.
        /// Date is first converted to UTC then adjusted.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static DateTime? ToTimeZoneTime(this DateTime? time, string timeZoneId = "Pakistan Standard Time")
        {
            if (time == null)
                return null;
            return time.Value.ToTimeZoneTime(timeZoneId);
        }

        public static DateTime? ToUTC(this DateTime? time, string timeZoneId)
        {
            if (time == null)
                return null;
            return time.Value.ToUTC(timeZoneId);
        }

        public static DateTime ToUTC(this DateTime time, string timeZoneId)
        {
            time = DateTime.SpecifyKind(time, DateTimeKind.Unspecified);
            var result = TimeZoneInfo.ConvertTimeToUtc(time, DateTimeHelper.GetTimesZone(timeZoneId));
            return result;
        }

        /// <summary>
        /// Returns TimeZone adjusted time for a given from a Utc or local time.
        /// Date is first converted to UTC then adjusted.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timeZoneInfo"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneTime(this DateTime time, TimeZoneInfo timeZoneInfo)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(time, timeZoneInfo);
        }

        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero) return dateTime; // Or could throw an ArgumentException
            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) return dateTime; // do not modify "guard" values
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
    }
}
