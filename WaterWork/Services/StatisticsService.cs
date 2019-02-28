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
        internal static double GetYearlyWorkedHours(ref WorkYear year)
        {
            double result = 0;
            foreach (WorkMonth workMonth in year.WorkMonths.Values)
            {
                foreach (WorkDay workDay in workMonth.WorkDays.Values)
                {
                    double minutesWorked = (workDay.EndTime - workDay.StartTime).TotalMinutes;
                    result += minutesWorked / 60;
                }
            }

            return result;
        }

        internal static double GetYearlyTotalHours(ref WorkYear year)
        {
            return year.OfficalWorkDayCount * 8;
        }
        
    }
}
