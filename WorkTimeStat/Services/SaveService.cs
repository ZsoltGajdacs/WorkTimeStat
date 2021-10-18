using System;
using System.Globalization;
using System.IO;
using System.Linq;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Services
{
    internal static class SaveService
    {
        /// <summary>
        /// Saves all data in a separate file, whose name includes that beginning and end dates it contains
        /// </summary>
        internal static void SaveYearlyBackupForReset()
        {
            UsageService.AddUsageForToday(GetTodaysUsageForSave());
            WorkKeeper keeper = WorkKeeper.Instance;
            DateTime firstRecordedDay = keeper.WorkDays.Keys.OrderBy(k => k.Date).First();
            DateTime today = DateTime.Now.Date;

            string saveDirPath = GetSaveDirPath();
            string backupFileName = string.Format(CultureInfo.CurrentCulture, "waterwork{0}-{1}.json",
                firstRecordedDay.Date.ToShortDateString(), today.Date.ToShortDateString());

            Serializer.JsonObjectSerialize(saveDirPath, backupFileName, ref keeper, DoBackup.Yes);
        }

        internal static void SaveData(SaveUsage saveUsage)
        {
            if (SaveUsage.Yes == saveUsage)
            {
                UsageService.AddUsageForToday(GetTodaysUsageForSave());
            }

            // Serialization stuff
            string saveDirPath = GetSaveDirPath();
            string waterWorkFileName = GetSaveFileName();
            WorkKeeper keeper = WorkKeeper.Instance;

            Serializer.JsonObjectSerialize(saveDirPath, waterWorkFileName, ref keeper, DoBackup.Yes);
        }

        internal static string GetSaveFileName()
        {
            return "worktimestat.json";
        }

        /// <summary>
        /// Gives back the location of the dir where all the files are saved
        /// </summary>
        /// <returns></returns>
        internal static string GetSaveDirPath()
        {
            FileInfo file = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            return file.DirectoryName;
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
