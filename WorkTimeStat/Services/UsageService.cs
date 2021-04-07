using System;
using System.Collections.Generic;
using UsageWatcher;
using UsageWatcher.Models;
using WorkTimeStat.Models;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Services
{
    internal static class UsageService
    {
        internal static void AddUsageForToday(TimeSpan usage)
        {
            WorkDay day = WorkDayService.GetCurrentDay();
            if (day != null && day.UsageTime < usage)
            {
                day.UsageTime = usage;
            }
        }

        internal static TimeSpan GetUsageForTimeframe(DateTime start, DateTime end)
        {
            IWatcher watcher = WorkKeeper.Instance.GetWatcher();
            return watcher.UsageTimeForGivenTimeframe(start, end);
        }

        internal static TimeSpan GetSavedUsageForDay(DateTime date)
        {
            WorkDay day = WorkDayService.GetDayAtDate(date);
            return day.UsageTime;
        }

        internal static List<UsageBlock> GetUsageListForTimeFrame(DateTime start, DateTime end)
        {
            IWatcher watcher = WorkKeeper.Instance.GetWatcher();
            return watcher.BlocksOfContinousUsageForTimeFrame(start, end);
        }

        internal static List<UsageBlock> GetBreaksInUsageListForTimeFrame(DateTime start, DateTime end)
        {
            IWatcher watcher = WorkKeeper.Instance.GetWatcher();
            return watcher.BreaksInContinousUsageForTimeFrame(start, end);
        }
    }
}
