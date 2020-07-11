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
            // If there is already data saved for this date, then there must have been a shutdown at some point
            // since this is so, the data that was collected since then is only valid for the day
            // if it's added to the already saved amount
            if (day.UsageTime != null)
            {
                day.UsageTime.Usage += usage;
            }
            else
            {
                day.UsageTime = new UsageTime(day.DayDate.Date, usage);
            }
        }

        internal static TimeSpan GetUsageForTimeframe(DateTime start, DateTime end, bool onlyNewData = false)
        {
            TimeSpan usage = GetLatestDataFromWatcher(start, end);

            if (!onlyNewData)
            {
                UsageTime savedUsage = GetSavedUsageForDay(start.Date);
                // If there is a saved usage for today then the app must have been stopped at some point,
                // since saving only occurs at shutdown. In this case the new usage data must be added to
                // the saved one
                if (savedUsage != null)
                {
                    usage += savedUsage.Usage;
                }
            }

            return usage;
        }

        internal static UsageTime GetSavedUsageForDay(DateTime date)
        {
            WorkDay day = WorkDayService.GetDayAtDate(date);
            return day.UsageTime;
        }

        private static TimeSpan GetLatestDataFromWatcher(DateTime start, DateTime end)
        {
            UsageWatcher.Watcher watcher = WorkKeeper.Instance.GetWatcher();
            return watcher.UsageForGivenTimeframe(start, end);
        }
    }
}
