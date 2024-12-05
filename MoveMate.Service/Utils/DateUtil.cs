using System.Text.RegularExpressions;
using MoveMate.Service.Commons;

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

        public static bool IsValidShard(string shard)
        {
            if (string.IsNullOrWhiteSpace(shard)) return false;

            string pattern = @"^(?:\d{4}(?:\d{2}(?:\d{2})?)?)$";

            return Regex.IsMatch(shard, pattern);
        }

        public static List<string> GenerateShardRange(string shardRange)
        {
            if (string.IsNullOrWhiteSpace(shardRange))
                shardRange = GetShardNow();

            var shards = shardRange.Split('-');

            if (shards.Length > 2)
                throw new ArgumentException(
                    "Invalid shard range format. Expected format: yyyy-yyyy, yyyyMM-yyyyMM, yyyyMMdd-yyyyMMdd, or a single shard.");

            var startShard = shards[0];
            var endShard =
                shards.Length == 2 ? shards[1] : shards[0]; // Nếu chỉ có 1 shard, startShard và endShard giống nhau

            if (!IsValidShard(startShard) || !IsValidShard(endShard))
                throw new ArgumentException("One or both shards are in an invalid format.");

            if (startShard.Length != endShard.Length)
                throw new ArgumentException(
                    "Start and end shards must be of the same type (yyyy, yyyyMM, or yyyyMMdd).");

            var result = new List<string>();

            if (startShard.Length == 4) // yyyy
            {
                if (!int.TryParse(startShard, out var startYear) || !int.TryParse(endShard, out var endYear))
                    throw new ArgumentException("Invalid year format.");

                for (var year = startYear; year <= endYear; year++)
                {
                    result.Add(year.ToString());
                }
            }
            else if (startShard.Length == 6) // yyyyMM
            {
                if (!DateTime.TryParseExact(startShard, "yyyyMM", null, System.Globalization.DateTimeStyles.None,
                        out var startDate) ||
                    !DateTime.TryParseExact(endShard, "yyyyMM", null, System.Globalization.DateTimeStyles.None,
                        out var endDate))
                    throw new ArgumentException("Invalid month format.");

                for (var date = startDate; date <= endDate; date = date.AddMonths(1))
                {
                    result.Add(date.ToString("yyyyMM"));
                }
            }
            else if (startShard.Length == 8) // yyyyMMdd
            {
                if (!DateTime.TryParseExact(startShard, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,
                        out var startDate) ||
                    !DateTime.TryParseExact(endShard, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,
                        out var endDate))
                    throw new ArgumentException("Invalid day format.");

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    result.Add(date.ToString("yyyyMMdd"));
                }
            }
            else
            {
                throw new ArgumentException("Unsupported shard format. Supported formats: yyyy, yyyyMM, yyyyMMdd.");
            }

            return result;
        }

        public static (bool isError, string msg, List<string> result) GenerateShardRangeV2(string shardRange)
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(shardRange))
            {
                return (true, "Shard range cannot be null or empty.", result);
            }

            var shards = shardRange.Split('-');

            if (shards.Length > 2)
            {
                return (true,
                    "Invalid shard range format. Expected format: yyyy-yyyy, yyyyMM-yyyyMM, yyyyMMdd-yyyyMMdd, or a single shard.",
                    result);
            }

            var startShard = shards[0];
            var endShard =
                shards.Length == 2 ? shards[1] : shards[0]; // Nếu chỉ có 1 shard, startShard và endShard giống nhau

            if (!IsValidShard(startShard) || !IsValidShard(endShard))
            {
                return (true, "One or both shards are in an invalid format.", result);
            }

            if (startShard.Length != endShard.Length)
            {
                return (true, "Start and end shards must be of the same type (yyyy, yyyyMM, or yyyyMMdd).", result);
            }

            if (startShard.Length == 4) // yyyy
            {
                if (!int.TryParse(startShard, out var startYear) || !int.TryParse(endShard, out var endYear))
                {
                    return (true, "Invalid year format.", result);
                }

                for (var year = startYear; year <= endYear; year++)
                {
                    result.Add(year.ToString());
                }
            }
            else if (startShard.Length == 6) // yyyyMM
            {
                if (!DateTime.TryParseExact(startShard, "yyyyMM", null, System.Globalization.DateTimeStyles.None,
                        out var startDate) ||
                    !DateTime.TryParseExact(endShard, "yyyyMM", null, System.Globalization.DateTimeStyles.None,
                        out var endDate))
                {
                    return (true, "Invalid month format.", result);
                }

                for (var date = startDate; date <= endDate; date = date.AddMonths(1))
                {
                    result.Add(date.ToString("yyyyMM"));
                }
            }
            else if (startShard.Length == 8) // yyyyMMdd
            {
                if (!DateTime.TryParseExact(startShard, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,
                        out var startDate) ||
                    !DateTime.TryParseExact(endShard, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,
                        out var endDate))
                {
                    return (true, "Invalid day format.", result);
                }

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    result.Add(date.ToString("yyyyMMdd"));
                }
            }
            else
            {
                return (true, "Unsupported shard format. Supported formats: yyyy, yyyyMM, yyyyMMdd.", result);
            }

            return (false, string.Empty, result); // Return no error, with the generated result
        }


        public static (bool isError, string msg, List<string> result) GenerateShardRangeV3(string shardRange)
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(shardRange))
            {
                return (true, MessageConstant.ShardErrorMessage.ShardRangeCannotBeNullOrEmpty, result);
            }

            var shards = shardRange.Split('-');

            if (shards.Length > 2)
            {
                return (true, MessageConstant.ShardErrorMessage.InvalidShardRangeFormat, result);
            }

            var startShard = shards[0];
            var endShard =
                shards.Length == 2 ? shards[1] : shards[0]; // Nếu chỉ có 1 shard, startShard và endShard giống nhau

            if (!IsValidShard(startShard) || !IsValidShard(endShard))
            {
                return (true, MessageConstant.ShardErrorMessage.InvalidShardFormat, result);
            }

            if (startShard.Length != endShard.Length)
            {
                return (true, MessageConstant.ShardErrorMessage.StartAndEndShardsMustBeSameType, result);
            }

            if (startShard.Length == 4) // yyyy
            {
                if (!int.TryParse(startShard, out var startYear) || !int.TryParse(endShard, out var endYear))
                {
                    return (true, MessageConstant.ShardErrorMessage.InvalidYearFormat, result);
                }

                for (var year = startYear; year <= endYear; year++)
                {
                    result.Add(year.ToString());
                }
            }
            else if (startShard.Length == 6) // yyyyMM
            {
                if (!DateTime.TryParseExact(startShard, "yyyyMM", null, System.Globalization.DateTimeStyles.None,
                        out var startDate) ||
                    !DateTime.TryParseExact(endShard, "yyyyMM", null, System.Globalization.DateTimeStyles.None,
                        out var endDate))
                {
                    return (true, MessageConstant.ShardErrorMessage.InvalidMonthFormat, result);
                }

                for (var date = startDate; date <= endDate; date = date.AddMonths(1))
                {
                    result.Add(date.ToString("yyyyMM"));
                }
            }
            else if (startShard.Length == 8) // yyyyMMdd
            {
                if (!DateTime.TryParseExact(startShard, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,
                        out var startDate) ||
                    !DateTime.TryParseExact(endShard, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,
                        out var endDate))
                {
                    return (true, MessageConstant.ShardErrorMessage.InvalidDayFormat, result);
                }

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    result.Add(date.ToString("yyyyMMdd"));
                }
            }
            else
            {
                return (true, MessageConstant.ShardErrorMessage.UnsupportedShardFormat, result);
            }

            return (false, string.Empty, result); // Return no error, with the generated result
        }


        public static string GetCurrentMonthShard()
        {
            return DateTime.Now.ToString("yyyyMM");
        }

        public static string GetCurrentMonthDaysShard()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var days = new List<string>();
            /*for (var date = startOfMonth; date <= endOfMonth; date = date.AddDays(1))
            {
                days.Add($"{date:yyyyMMdd}");
            }*/
            days.Add($"{startOfMonth:yyyyMMdd}");
            days.Add($"{endOfMonth:yyyyMMdd}");

            return string.Join("-", days);
        }


        public static string GetCurrentWeekDaysShard()
        {
            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + 1);
            var endOfWeek = startOfWeek.AddDays(6);

            var days = new List<string>();
            /*for (var date = startOfWeek; date <= endOfWeek; date = date.AddDays(1))
            {
                days.Add($"{date:yyyyMMdd}");
            }*/
            days.Add($"{startOfWeek:yyyyMMdd}");
            days.Add($"{endOfWeek:yyyyMMdd}");
            
            return string.Join("-", days);
        }
    }
}