using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UsageWatcher;
using UsageWatcher.Enums;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Services;

namespace WorkTimeStat.Storage
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class WorkKeeper : IDisposable
    {
        [NonSerialized]
        private IWatcher watcher;

        public Dictionary<DateTime, WorkDay> WorkDays { get; private set; }
        public Dictionary<int, int> DaysWorkedInMonth { get; private set; }
        public List<DateTime> LeaveDays { get; set; }
        public List<DateTime> SickDays { get; set; }
        public Dictionary<DateTime, List<TicketTime>> WorkedTickets { get; private set; }
        public WorkSettings Settings { get; set; }

        public IWatcher GetWatcher()
        {
            return watcher;
        }

        internal void Init()
        {
            watcher = new Watcher("WorktimeStat", Settings.WatcherResolution,
                Settings.WatcherSavePreference, Settings.WatcherDataPrecision);

            CheckForHolidayYearsEnd();
        }

        private void CheckForHolidayYearsEnd()
        {
            DateTime today = DateTime.Today.Date;
            if (today.Year != Settings.HolidayYearStart.Year && today >= Settings.HolidayYearStart.AddYears(1))
            {
                DoYearlyReset();
                Settings.HolidayYearStart = today;
            }
        }

        private void DoYearlyReset()
        {
            SaveService.SaveYearlyBackupForReset();
            KeeperDto dto = WorkDayService.FindDataBeforeDate(DateTime.Today.Date);

            WorkDays.Clear();
            DaysWorkedInMonth.Clear();
            LeaveDays.Clear();
            SickDays.Clear();

            if (dto != null)
            {
                WorkDays = dto.WorkDays;
                LeaveDays = dto.LeaveDays;
                SickDays = dto.SickDays;
            }
        }

        internal List<TicketTime> GetTodaysTickets()
        {
            WorkedTickets.TryGetValue(DateTime.Today, out List<TicketTime> tickets);
            return tickets;
        }

        internal void UpdateTicketList(List<TicketTime> tickets)
        {
            if (WorkedTickets == null)
            {
                WorkedTickets = new Dictionary<DateTime, List<TicketTime>>();
            }

            WorkedTickets.TryGetValue(DateTime.Today, out List<TicketTime> storedTickets);
            if (storedTickets == null)
            {
                storedTickets = new List<TicketTime>();
                WorkedTickets.Add(DateTime.Today, storedTickets);
            }

            storedTickets.Clear();
            storedTickets.AddRange(tickets);
        }

        #region Singleton
        public static WorkKeeper Instance { get { return lazy.Value; } }

        private WorkKeeper()
        {
            WorkDays = new Dictionary<DateTime, WorkDay>();
            DaysWorkedInMonth = new Dictionary<int, int>();
            LeaveDays = new List<DateTime>();
            SickDays = new List<DateTime>();
            WorkedTickets = new Dictionary<DateTime, List<TicketTime>>();

            Settings = new WorkSettings
            {
                WatcherResolution = Resolution.TwoMinutes,
                WatcherSavePreference = SavePreference.KeepDataForAYear,
                WatcherDataPrecision = DataPrecision.High,
                HolidayYearStart = DateTime.Today
            };
        }

        private static readonly Lazy<WorkKeeper> lazy = new Lazy<WorkKeeper>(() =>
        {
            WorkKeeper workKeeper = Serializer.JsonObjectDeserialize<WorkKeeper>(
                SaveService.GetSaveDirPath(), SaveService.GetSaveFileName());

            return workKeeper ?? new WorkKeeper();
        });

        #endregion

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //  dispose managed state (managed objects).
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.
                watcher.Dispose();
                disposedValue = true;
            }
        }

        ~WorkKeeper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
