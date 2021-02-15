using Newtonsoft.Json;
using System;
using System.ComponentModel;
using UsageWatcher.Enums;
using WorkTimeStat.Enums;

namespace WorkTimeStat.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class WorkSettings : INotifyPropertyChanged
    {
        public Boolean IsLunchTimeWorkTimeDefault { get; set; }
        public decimal AmountOfLitreInOneUnit { get; set; }
        public int YearlyLeaveNumber { get; set; }
        public double DailyWorkHours { get; set; }
        public Resolution WatcherResolution { get; set; }
        public DataPrecision WatcherDataPrecision { get; set; }
        public SavePreference WatcherSavePreference { get; set; }
        public WorkPlaceType WorkPlaceType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
