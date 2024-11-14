using MoveMate.Service.ViewModels.ModelRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Utils
{
    public static class DateUtil
    {
        public enum TypeCheck
        {
            HOUR,
            MINUTE,
        }

        public static DateTime ConvertUnixTimeToDateTime(long utcExpiredDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval = dateTimeInterval.AddSeconds(utcExpiredDate).ToUniversalTime();
            return dateTimeInterval;
        }

        public static bool IsTimeUpdateValid(TimeSpan timeSpanLater, TimeSpan timeSpanEarlier, int condition,
            TypeCheck type)
        {
            // Subtract the two TimeSpan objects to get the difference.
            TimeSpan difference = timeSpanLater.Subtract(timeSpanEarlier);

            // Check if the difference is at least condition hour.
            if (type == TypeCheck.HOUR)
            {
                return difference.TotalHours >= condition;
            }

            // Check if the difference is at least condition minute.
            return difference.TotalMinutes >= condition;
        }

        public static void AddDateToDictionary(out Dictionary<DateTime, decimal> dates)
        {
            dates = new Dictionary<DateTime, decimal>();
            for (var i = 0; i <= 6; i++)
            {
                if (i == 0)
                {
                    dates.Add(DateTime.Now.Date, 0);
                    continue;
                }

                dates.Add(DateTime.Now.AddDays(-i).Date, 0);
            }
        }

        public static DateTime ConvertStringToDateTime(string date)
        {
            return DateTime.ParseExact(date, "dd/MM/yyyy", null);
        }

        public static bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static bool IsHoliday(DateTime date)
        {
            List<DateTime> holidays = GetVietnamHolidays(date.Year);
            return holidays.Any(holiday => holiday.Date == date.Date);
        }

        public static long GetTimeStamp(DateTime date)
        {
            return (long)(date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }

        public static long GetTimeStamp()
        {
            return GetTimeStamp(DateTime.Now);
        }

        public static string GetDateStr()
        {
            return DateTime.Now.ToString("yyMMdd");
        }

        public static bool IsOutsideBusinessHours(DateTime date)
        {
            TimeSpan startBusinessHours = new TimeSpan(8, 0, 0); // 08:00 AM
            TimeSpan endBusinessHours = new TimeSpan(17, 0, 0); // 05:00 PM

            return date.TimeOfDay < startBusinessHours ||
                   date.TimeOfDay >= endBusinessHours;
        }

        private static List<DateTime> GetVietnamHolidays(int year)
        {
            return new List<DateTime>
            {
                new DateTime(year, 1, 1),
                GetHungKingsCommemorationDay(year),
                new DateTime(year, 4, 30),
                new DateTime(year, 5, 1),
                new DateTime(year, 9, 2)
            };
        }

        private static DateTime GetHungKingsCommemorationDay(int year)
        {
            return new DateTime(year, 4, 18);
        }

        public static DateTime GetCurrentSEATime()
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            DateTime localTime = DateTime.Now;
            DateTime utcTime = TimeZoneInfo.ConvertTime(localTime, TimeZoneInfo.Local, tz);
            return utcTime;
        }

        public static DateTime ConvertToSEATime(DateTime value)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            DateTime convertedTime = TimeZoneInfo.ConvertTime(value, tz);
            return convertedTime;
        }

        public static TimeZoneInfo GetSEATimeZone()
        {
            TimeZoneInfo tz;
            try
            {
                // Try using Windows time zone
                tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                // Fallback to IANA time zone for Linux/Docker
                tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }

            return tz;
        }

        public static String GetShardNow()
        {
            string dateKey = DateTime.Now.ToString("yyyyMMdd");
            return dateKey;
        }

        public static DateTime GetDateFormat(DateTime dateTime)
        {
            string formattedDate = dateTime.ToString("yyyyMMdd");
            return DateTime.ParseExact(formattedDate, "yyyyMMdd", null);
        }


        public static String GetShard(DateTime? time)
        {
            string dateKey = time.HasValue ? time.Value.ToString("yyyyMMdd") : GetShardNow();
            return dateKey;
        }

        public static String GetKeyReview()
        {
            string dateKey = GetShardNow();
            string redisKey = $"reviewerQueue_{dateKey}";

            return redisKey;
        }
        
        public static String GetKeyReview(int groupId, int scheduleId)
        {
            string dateKey = GetShardNow();
            string redisKey = $"reviewerQueue_{dateKey}_{groupId}_{scheduleId}";

            return redisKey;
        }

        public static String GetKeyDriver(DateTime? time, int truckCateId)
        {
            string dateKey = GetShard(time);
            string redisKey = $"driverQueue_{truckCateId}_{dateKey}";

            return redisKey;
        }
        public static String GetKeyDriverV2(DateTime? time, int truckCateId)
        {
            string dateKey = GetShard(time);
            string redisKey = $"driverQueueV2_{truckCateId}_{dateKey}";

            return redisKey;
        }
        
        public static String GetKeyDriver(DateTime? time, int truckCateId, int groupId, int scheduleId)
        {
            string dateKey = GetShard(time);
            string redisKey = $"driverQueue_{truckCateId}_{dateKey}_{groupId}_{scheduleId}";

            return redisKey;
        }
        public static String GetKeyDriverV2(DateTime? time, int truckCateId, int groupId, int scheduleId)
        {
            string dateKey = GetShard(time);
            string redisKey = $"driverQueueV2_{truckCateId}_{dateKey}_{groupId}_{scheduleId}";

            return redisKey;
        }
        
        public static String GetKeyDriverBooking(DateTime? time, int bookingId)
        {
            string dateKey = GetShard(time);
            string redisKey = $"driver_{dateKey}_{bookingId}";

            return redisKey;
        }
        
        // porter
        public static String GetKeyPorter(DateTime? time)
        {
            string dateKey = GetShard(time);
            string redisKey = $"porterQueue_{dateKey}";

            return redisKey;
        }
        public static String GetKeyPorterV2(DateTime? time)
        {
            string dateKey = GetShard(time);
            string redisKey = $"porterQueueV2_{dateKey}";

            return redisKey;
        }
        
        public static String GetKeyPorterBooking(DateTime? time, int bookingId)
        {
            string dateKey = GetShard(time);
            string redisKey = $"porter_{dateKey}_{bookingId}";

            return redisKey;
        }

        public static TimeSpan TimeUntilEndOfDay(DateTime inputDateTime)
        {
            DateTime endOfDay = inputDateTime.Date.AddDays(2).AddTicks(-1);

            TimeSpan timeUntilEndOfDay = endOfDay - DateTime.Now;

            return timeUntilEndOfDay;
        }

        public static bool IsAtLeast24HoursApart(DateTime startTime, DateTime endTime)
        {
            return (endTime - startTime).TotalHours >= 24;
        }

    }
}