using System;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Services
{
    internal static class SaveService
    {
        internal static void SaveData(SaveUsage saveUsage)
        {
            if (SaveUsage.Yes == saveUsage)
            {
                UsageService.AddUsageForToday(GetTodaysUsageForSave());
            }

            // Serialization stuff
            string saveDirPath = FilesLocation.GetSaveDirPath();
            string waterWorkFileName = FilesLocation.GetSaveFileName();
            WorkKeeper keeper = WorkKeeper.Instance;

            Serializer.JsonObjectSerialize(saveDirPath, waterWorkFileName, ref keeper, DoBackup.Yes);
        }

        private static TimeSpan GetTodaysUsageForSave()
        {
            WorkDay today = WorkDayService.GetCurrentDay();

            TimeSpan usage = default;
            if (today != null)
            {
                DateTime start = today.DayDate.Date + today.StartTime;
                DateTime end = today.DayDate.Date + today.EndTime;

                usage = UsageService.GetUsageForTimeframe(start, end);
            }


            return usage;
        }
    }

    internal enum SaveUsage
    {
        Yes,
        No
    }
}
