using System;
using System.Collections.Generic;
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
            bool isPresent = keeper.WorkDays.TryGetValue(date.Date, out WorkDay day);

            return isPresent ? day : null;
        }

        internal static WorkDay GetCurrentDay()
        {
            DateTime today = DateTime.Now.Date;
            return GetDayAtDate(today);
        }

        internal static WorkDay GetLastWorkDay()
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            DateTime now = DateTime.Now.Date;

            return keeper.WorkDays
                                    .Where(d => d.Key.Date != now.Date && !StatisticsService.IsDayOverworkOnlyDay(d.Value))
                                    .OrderByDescending(d => d.Key.Date)
                                    .Select(d => d.Value)
                                    .FirstOrDefault();
        }

        /// <summary>
        /// Returns all the data kept that is dated before today
        /// </summary>
        /// <param name="date">The date before which data is searched</param>
        internal static KeeperDto FindDataBeforeDate(DateTime date)
        {
            WorkKeeper keeper = WorkKeeper.Instance;
            KeeperDto keeperDto = new KeeperDto
            {
                WorkDays = keeper.WorkDays.Where(d => d.Key < date).ToDictionary(k => k.Key, v => v.Value),
                LeaveDays = keeper.LeaveDays.Where(l => l.Date < date.Date).ToList(),
                SickDays = keeper.SickDays.Where(s => s.Date < date.Date).ToList()
            };

            return keeperDto;
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
