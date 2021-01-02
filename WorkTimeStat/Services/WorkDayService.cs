using System;
using System.Linq;
using WorkTimeStat.Models;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Services
{
    internal static class WorkDayService
    {
        internal static WorkDay GetDayAtDate(DateTime date)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            bool isPresent = keeper.WorkDays.TryGetValue(date.Date, out WorkDay ma);

            return isPresent ? ma : null;
        }

        internal static WorkDay GetCurrentDay()
        {
            DateTime today = DateTime.Now.Date;
            return GetDayAtDate(today);
        }

        internal static WorkDay GetYesterWorkDay()
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            DateTime now = DateTime.Now.Date;
            return keeper.WorkDays.Where(d => d.Key.Date != now.Date)
                                    .OrderByDescending(d => d.Key.Date)
                                    .Select(d => d.Value)
                                    .FirstOrDefault();
        }

        internal static void SetDayAtDate(DateTime date, ref WorkDay day)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            keeper.WorkDays[date] = day;
        }

        internal static void SetCurrentDay(ref WorkDay today)
        {
            DateTime todayDate = DateTime.Now.Date;
            SetDayAtDate(todayDate, ref today);
        }
    }
}
