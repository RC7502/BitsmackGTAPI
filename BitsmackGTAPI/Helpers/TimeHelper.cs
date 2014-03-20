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

        public static DateTime GetNextDayOfWeek(DateTime start, DayOfWeek dayOfWeek)
        {
            int daysToAdd = ((int) dayOfWeek - (int) start.DayOfWeek + 7)%7;
            return start.AddDays(daysToAdd);
        }

        public static TimeSpan StringPlusTZToTimeSpan(string time)
        {
            return new TimeSpan(Convert.ToInt32(time.Substring(0, 2)),Convert.ToInt32(time.Substring(3, 2)), 0);
        }

        public static double GetBusinessDays(DateTime startD, DateTime endD)
        {
            double calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if ((int)endD.DayOfWeek == 6) calcBusinessDays--;
            if ((int)startD.DayOfWeek == 0) calcBusinessDays--;

            return calcBusinessDays;
        }

        public static string DateToMonthYear(DateTime parmDate)
        {
            return string.Format(parmDate.Month < 10 ? "{0}0{1}" : "{0}{1}", parmDate.Year, parmDate.Month);
        }

        public static string MinutesToHours(int minutes)
        {
            return TimeSpan.FromMinutes(minutes).ToString(@"hh\:mm");
        }

        public static string SecondsToTime(double seconds)
        {
            return TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
        }

        public static DateTime MillisecondsToDate(long milli)
        {
            var time = TimeSpan.FromMilliseconds(milli);
            var result = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return result.Add(time);

        }
    }
}