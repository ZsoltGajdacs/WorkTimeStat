using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        internal static double GetYearlyTotalHours(ref WorkYear year)
        {
            return year.NoOfDaysWorked * 8;
        }
        #endregion

        #region Month counters
        internal static double GetMonthlyWorkedHours(WorkMonth month)
        {
            double result = 0;
            foreach (WorkDay workDay in month.WorkDays.Values)
            {
                double minutesWorked = (workDay.EndTime - workDay.StartTime).TotalMinutes;

                if (!workDay.IsLunchTimeWorkTime)
                {
                    minutesWorked -= workDay.LunchBreakDuration; 
                }

                result += minutesWorked / 60;
            }

            return result;
        }

        internal static double GetMonthlyTotalHours(WorkMonth month)
        {
            return month.NoOfDaysWorked * 8;
        }
        #endregion

        #region Helpers
        internal static int GetTodayNum()
        {
            return int.Parse(DateTime.Today.Date.ToString("dd"));
        }

        internal static int GetThisMonthNum()
        {
            return int.Parse(DateTime.Today.Date.ToString("MM"));
        }

        internal static int GetThisYearNum()
        {
            return int.Parse(DateTime.Now.Year.ToString());
        }
        #endregion

    }
}
