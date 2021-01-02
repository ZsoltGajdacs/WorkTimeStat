using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WorkTimeStat.Enums;
using WorkTimeStat.Helpers;

namespace WorkTimeStat.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class WorkDay : INotifyPropertyChanged
    {
        public DateTime DayDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan UsageTime { get; set; }
        public TimeSpan TotalDailyUsage { get; set; }
        public int LunchBreakDuration { get; set; }
        public int OtherBreakDuration { get; set; }
        public int OverWorkDuration { get; set; }
        public WorkDayType WorkDayType { get; set; }
        public decimal WaterConsumptionCount { get; set; }
        public decimal AmountOfLitreInOneUnit { get; set; }
        public bool IsLunchTimeWorkTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #region CTORS
        public WorkDay(bool isLunchTimeWorkTime, decimal amountOfLiterInOneUnit, double dailyWorkHours)
        {
            AmountOfLitreInOneUnit = amountOfLiterInOneUnit;
            IsLunchTimeWorkTime = isLunchTimeWorkTime;
            DayDate = DateTime.Now.Date;
            StartTime = NumberFormatter.RoundUpTime(DateTime.Now, TimeSpan.FromMinutes(15)).TimeOfDay;
            EndTime = StartTime + TimeSpan.FromHours(dailyWorkHours);
            WorkDayType = WorkDayType.NORMAL;
        }
        #endregion

        internal void IncreaseWaterConsumption()
        {
            WaterConsumptionCount = Decimal.Add(WaterConsumptionCount, AmountOfLitreInOneUnit);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
