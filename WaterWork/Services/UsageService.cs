using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterWork.Models;
using WaterWork.Storage;

namespace WaterWork.Services
{
    internal static class UsageService
    {
        internal static void AddUsage(DateTime day, TimeSpan usage)
        {
            var keeper = WorkKeeper.Instance;
            
            UsageTime savedTime = UsageHistory.Where(u => u.Day.Date == day.Date).FirstOrDefault();
            // If there is already data saved for this date, then there must have been a shutdown at some point
            // since this is so, the data that was collected since then is only valid for the day
            // if it's added to the already saved amount
            if (savedTime != null)
            {
                savedTime.Usage += usage;
            }
            else
            {
                UsageTime newTime = new UsageTime(day, usage);
                UsageHistory.Add(newTime);
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

        private static TimeSpan GetLatestDataFromWatcher(DateTime start, DateTime end)
        {
            return watcher.UsageForGivenTimeframe(start, end);
        }

        private static UsageTime GetSavedUsageForDay(DateTime date)
        {
            UsageTime time = UsageHistory.Where(u => u.Day.Date == date.Date).FirstOrDefault();
            return time;
        }
    }
}
