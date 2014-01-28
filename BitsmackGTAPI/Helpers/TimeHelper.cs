using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Helpers
{
    public class TimeHelper
    {
        public static DateTime CopyDateTime(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
        }

        public static DateTime ConvertUtcToLocal(DateTime date)
        {
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(date, est);

        }
    }
}