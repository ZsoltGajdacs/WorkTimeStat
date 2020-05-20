using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UsageWatcher;

namespace WaterWork.Storage
{
    internal class UsageKeeper : IDisposable
    {
        private Watcher watcher;

        internal TimeSpan GetWatchedUsage()
        {
            return watcher.UsageSoFar();
        }

        #region Singleton stuff
        private UsageKeeper()
        {
            watcher = new Watcher(Resolution.TWO_MINUTES);
        }

        private static readonly Lazy<UsageKeeper> lazy = new Lazy<UsageKeeper>(() =>
        {
            /*string path = FilesLocation.GetSaveDirPath() + FilesLocation.GetWorkLogFileName();
            UsageKeeper usageKeeper = Serializer.JsonObjectDeserialize<LogKeeper>(path);

            // Fill active list from archives
            if (logKeeper != null)
            {
                List<LogEntry> activeEntries = logKeeper.WorkLogs.Values.Where(w => w.IsFinished == false).ToList();
                logKeeper.ActiveWorkLogs = new BindingList<LogEntry>(activeEntries);
            }*/

            return new UsageKeeper();
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
