using System;
using WaterWork.Helpers;
using WaterWork.Models;
using WaterWork.Storage;

namespace WaterWork.Services
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
            string waterWorkFileName = FilesLocation.GetWaterWorkFileName();
            WorkKeeper keeper = WorkKeeper.Instance;

            Serializer.JsonObjectSerialize(saveDirPath, waterWorkFileName, ref keeper, DoBackup.Yes);
        }

        private static TimeSpan GetTodaysUsageForSave()
        {
            WorkDay today = WorkDayService.GetCurrentDay();
            DateTime start = today.DayDate.Date + today.StartTime;
            DateTime end = today.DayDate.Date + today.EndTime;

            return UsageService.GetUsageForTimeframe(start, end);
        }
    }

    internal enum SaveUsage
    {
        Yes,
        No
    }
}
