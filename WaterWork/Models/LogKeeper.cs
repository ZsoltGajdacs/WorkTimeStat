using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WaterWork.Helpers;

namespace WaterWork.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class LogKeeper : INotifyPropertyChanged, IDisposable
    {
        // Constants
        private static readonly long TICK_TIME = 5 * 60 * 1000; // 5 minutes

        // Lists
        public Dictionary<string, LogEntry> WorkLogs { get; set; }
        [JsonIgnore]
        public BindingList<LogEntry> ActiveWorkLogs { get; set; }

        // Time stuff
        [NonSerialized]
        private Timer timer;
        private DateTime lastTickTime;

        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        #region Singleton stuff
        private LogKeeper()
        {
            WorkLogs = new Dictionary<string, LogEntry>();
            ActiveWorkLogs = new BindingList<LogEntry>();

            // Get current dateTime
            lastTickTime = DateTime.Now;

            // Tick every 5 minutes
            timer = new Timer(TICK_TIME);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private static readonly Lazy<LogKeeper> lazy = new Lazy<LogKeeper>(() =>
        {
            string path = FilesLocation.GetSaveDirPath() + FilesLocation.GetWorkLogFileName();
            LogKeeper logkeeper = Serializer.JsonObjectDeserialize<LogKeeper>(path);

            // Fill active list from archives
            List<LogEntry> activeEntries = logkeeper.WorkLogs.Values.Where(w => w.IsFinished == false).ToList();
            logkeeper.ActiveWorkLogs = new BindingList<LogEntry>(activeEntries);

            return logkeeper ?? new LogKeeper();
        });

        public static LogKeeper Instance { get { return lazy.Value; } }
        #endregion

        #region Event handlers
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lastTickTime = DateTime.Now;

            foreach (LogEntry entry in ActiveWorkLogs)
            {
                // Set paused state
                if (!entry.IsPaused)
                {
                    entry.TimeSpent += TimeSpan.FromMilliseconds(TICK_TIME);
                    WorkLogs[entry.LogName].TimeSpent += TimeSpan.FromMilliseconds(TICK_TIME);

                    WorkLogs[entry.LogName].IsPaused = false;
                }
                else
                {
                    WorkLogs[entry.LogName].IsPaused = true;
                }

                // Set finished state
                if (entry.IsFinished)
                {
                    WorkLogs[entry.LogName].IsFinished = true;
                }
                else
                {
                    WorkLogs[entry.LogName].IsFinished = false;
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            ((IDisposable)timer).Dispose();
        }
        #endregion
    }
}
