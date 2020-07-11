using System;
using System.Linq;
using WaterWork.Models;
using WaterWork.Storage;

namespace WaterWork.Services
{
    internal static class StatisticsService
    {
        internal static void FullReCountWorkedDays()
        {
            for (int monthNum = 1; monthNum < 13; ++monthNum)
            {
                CountWorkedDaysInMonth(monthNum);
            }
        }

        #region Month counters
        internal static void CountWorkedDaysInMonth(int month)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            int workedDays = keeper.WorkDays.Where(d => d.Key.Date.Month == month).Count();
            keeper.DaysWorkedInMonth[month] = workedDays;
        }

        internal static double GetMonthlyWorkedHours(int month)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            System.Collections.Generic.IEnumerable<WorkDay> workDaysInMonth = keeper.WorkDays.Where(d => d.Key.Month == month)
                                                    .Select(d => d.Value);
            double result = 0;
            foreach (WorkDay workDay in workDaysInMonth)
            {

                result += GetDailyWorkedHours(workDay);
            }

            return RoundToMidWithTwoPrecision(result);
        }

        internal static double GetMonthlyTotalHours(int month, double workdayHourLength)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            return keeper.DaysWorkedInMonth[month] * workdayHourLength;
        }
        #endregion

        #region Day Counters
        internal static double GetDailyWorkedHours(WorkDay day)
        {
            if (day == null)
            {
                return 0;
            }

            double minutesWorked = (day.EndTime - day.StartTime).TotalMinutes;

            minutesWorked -= day.OtherBreakDuration;
            minutesWorked -= day.OverWorkDuration;

            if (!day.IsLunchTimeWorkTime)
            {
                minutesWorked -= day.LunchBreakDuration;
            }

            return minutesWorked / 60;
        }
        #endregion

        #region UsageCounter
        /// <summary>
        /// Gives back the usage data for the given day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        internal static double GetUsageForDay(ref WorkDay day)
        {
            if (day == null)
            {
                return 0.0;
            }

            if (day.DayDate != DateTime.Today)
            {
                DateTime startDate = day.DayDate.Date + day.StartTime;
                DateTime endDate = day.DayDate.Date + day.EndTime;

                TimeSpan usageInTimeframe = UsageService.GetUsageForTimeframe(startDate, endDate);
                return RoundToMidWithTwoPrecision(usageInTimeframe.TotalHours);
            }
            else
            {
                return GetUsageForToday();
            }
        }

        internal static double GetUsageForToday()
        {
            WorkDay day = WorkDayService.GetCurrentDay();
            DateTime startDate = day.DayDate.Date + day.StartTime;
            DateTime endDate = day.DayDate.Date + day.EndTime;

            TimeSpan usageInTimeframe = UsageService.GetUsageForTimeframe(startDate, endDate);
            return RoundToMidWithTwoPrecision(usageInTimeframe.TotalHours);
        }

        internal static double GetUsageForMonth(int month)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            System.Collections.Generic.IEnumerable<UsageTime> usagesInMonth = keeper.WorkDays.Where(d => d.Key.Month == month)
                                                .Select(d => d.Value.UsageTime);
            double result = 0;
            foreach (UsageTime usage in usagesInMonth)
            {
                if (usage != null)
                {
                    result += usage.Usage.TotalHours;
                }
            }

            return RoundToMidWithTwoPrecision(result);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Gives back the midpoint rounded number with two digits after zero for the given double
        /// </summary>
        private static double RoundToMidWithTwoPrecision(double num)
        {
            return Math.Round(num, 2, MidpointRounding.ToEven);
        }
        #endregion

    }
}
