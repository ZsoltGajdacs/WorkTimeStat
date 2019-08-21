using System;
using WaterWork.Models;

namespace WaterWork.Services
{
    internal static class StatisticsService
    {
        #region Year counters
        internal static double GetYearlyWorkedHours(ref WorkYear year)
        {
            double result = 0;
            foreach (WorkMonth workMonth in year.WorkMonths.Values)
            {
                result += GetMonthlyWorkedHours(workMonth);
            }

            return result;
        }

        internal static double GetYearlyTotalHours(ref WorkYear year, double workdayHourLength)
        {
            return year.NoOfDaysWorked * workdayHourLength;
        }
        #endregion

        #region Month counters
        internal static double GetMonthlyWorkedHours(WorkMonth month)
        {
            double result = 0;
            foreach (WorkDay workDay in month.WorkDays.Values)
            {

                result += GetDailyWorkedHours(workDay);
            }

            return result;
        }

        internal static double GetMonthlyTotalHours(WorkMonth month, double workdayHourLength)
        {
            return month.NoOfDaysWorked * workdayHourLength;
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

        #region Helpers
        internal static int GetTodayNum()
        {
            return GetDayForDate(DateTime.Today);
        }

        internal static int GetThisMonthNum()
        {
            return GetMonthForDate(DateTime.Today);
        }

        internal static int GetThisYearNum()
        {
            return GetYearForDate(DateTime.Today);
        }

        internal static int GetYearForDate(DateTime date)
        {
            return int.Parse(date.ToString("yyyy"));
        }

        internal static int GetMonthForDate(DateTime date)
        {
            return int.Parse(date.ToString("MM"));
        }

        internal static int GetDayForDate(DateTime date)
        {
            return int.Parse(date.ToString("dd"));
        }
        #endregion

    }
}
