﻿using System;
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
    }
}