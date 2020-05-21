using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UsageWatcher;
using WaterWork.Helpers;
using WaterWork.Models;

namespace WaterWork.Storage
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class UsageKeeper : IDisposable
    {
        [NonSerialized]
        private readonly Watcher watcher;

        public List<UsageTime> UsageHistory { get; private set; }

        internal TimeSpan GetUsageForTimeframe(DateTime start, DateTime end, bool onlyNewData = false)
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

        internal void AddUsage(DateTime day, TimeSpan usage)
        {
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

        private TimeSpan GetLatestDataFromWatcher(DateTime start, DateTime end)
        {
            return watcher.UsageForGivenTimeframe(start, end);
        }

        private UsageTime GetSavedUsageForDay(DateTime date)
        {
            UsageTime time = UsageHistory.Where(u => u.Day.Date == date.Date).FirstOrDefault();
            return time;
        }

        #region Singleton stuff
        private UsageKeeper()
        {
            watcher = new Watcher(Resolution.TWO_MINUTES);
            UsageHistory = new List<UsageTime>();
        }

        private static readonly Lazy<UsageKeeper> lazy = new Lazy<UsageKeeper>(() =>
        {
            string path = FilesLocation.GetSaveDirPath() + FilesLocation.GetUsageLogName();
            UsageKeeper usageKeeper = Serializer.JsonObjectDeserialize<UsageKeeper>(path);

            // Fill active list from archives
            if (usageKeeper != null)
            {
                List<UsageTime> usageTransfer = usageKeeper.UsageHistory;
                usageKeeper = new UsageKeeper();
                usageKeeper.UsageHistory = usageTransfer;
            }

            return usageKeeper ?? new UsageKeeper();
        });

        public static UsageKeeper Instance { get { return lazy.Value; } }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                watcher.Dispose();
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UsageKeeper()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
