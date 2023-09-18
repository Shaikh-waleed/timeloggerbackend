using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace TimeLogger.Infrastructure.Utility.Helpers
{
    public static class DateTimeHelper
    {
        public static TimeZoneInfo GetTimesZone(string Id)
        {
            return GetTimesZones().Where(c => c.Id.Equals(Id)).FirstOrDefault();
        }

        public static ReadOnlyCollection<TimeZoneInfo> GetTimesZones()
        {
            return TimeZoneInfo.GetSystemTimeZones();
        }
    }
}
