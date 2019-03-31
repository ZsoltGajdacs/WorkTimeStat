using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WaterWork.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class LogKeeper : INotifyPropertyChanged
    {
        public BindingList<LogEntry> WorkLogs { get; set; }
        private Dictionary<string, Stopwatch> Watches { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public LogKeeper()
        {
            WorkLogs = new BindingList<LogEntry>();
            WorkLogs.AddingNew += WorkLogs_AddingNew;
        }

        #region Event handlers
        private void WorkLogs_AddingNew(object sender, AddingNewEventArgs e)
        {
            LogEntry logEntry = (LogEntry)e.NewObject;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Watches.Add(logEntry.LogName, watch);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
