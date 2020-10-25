using System;
using WaterWork.Models;
using WaterWork.Storage;

namespace WaterWork.Services
{
    internal static class UsageService
    {
        internal static void AddUsageForToday(TimeSpan usage)
        {
            WorkDay day = WorkDayService.GetCurrentDay();
            if (day.UsageTime < usage)
            {
                day.UsageTime = usage;
            }

        }

        internal static TimeSpan GetUsageForTimeframe(DateTime start, DateTime end)
        {
            UsageWatcher.IWatcher watcher = WorkKeeper.Instance.GetWatcher();
            return watcher.UsageForGivenTimeframe(start, end);
        }

        internal static TimeSpan GetSavedUsageForDay(DateTime date)
        {
            WorkDay day = WorkDayService.GetDayAtDate(date);
            return day.UsageTime;
        }
    }
}
