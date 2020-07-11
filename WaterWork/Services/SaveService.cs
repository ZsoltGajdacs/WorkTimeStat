using System;
using WaterWork.Helpers;
using WaterWork.Models;
using WaterWork.Storage;

namespace WaterWork.Services
{
    internal static class SaveService
    {
        internal static void SaveData(bool saveUsage)
        {
            if (saveUsage)
            {
                // Usage needs to be saved here, becuase this is the only place
                // where I can be certain that I have the complete time
                UsageService.AddUsageForToday(GetTodaysUsageForSave());
            }

            // Serialization stuff
            string saveDirPath = FilesLocation.GetSaveDirPath();
            string waterWorkFileName = FilesLocation.GetWaterWorkFileName();
            WorkKeeper keeper = WorkKeeper.Instance;

            Serializer.JsonObjectSerialize(saveDirPath + waterWorkFileName, ref keeper, true);
        }

        private static TimeSpan GetTodaysUsageForSave()
        {
            WorkDay today = WorkDayService.GetCurrentDay();
            DateTime start = today.DayDate.Date + today.StartTime;
            DateTime end = today.DayDate.Date + today.EndTime;

            return UsageService.GetUsageForTimeframe(start, end, true);
        }
    }
}
