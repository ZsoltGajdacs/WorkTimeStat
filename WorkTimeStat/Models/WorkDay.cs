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
        public WorkPlaceType WorkPlaceType { get; set; }
        public decimal WaterConsumptionCount { get; set; }
        public decimal AmountOfLitreInOneUnit { get; set; }
        public bool IsLunchTimeWorkTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #region CTORS
        public WorkDay(bool isLunchTimeWorkTime, decimal amountOfLiterInOneUnit, 
                                    double dailyWorkHours, WorkPlaceType workPlaceType)
        {
            AmountOfLitreInOneUnit = amountOfLiterInOneUnit;
            IsLunchTimeWorkTime = isLunchTimeWorkTime;
            DayDate = DateTime.Now.Date;
            StartTime = NumberFormatter.RoundUpTime(DateTime.Now, TimeSpan.FromMinutes(15)).TimeOfDay;
            EndTime = StartTime + TimeSpan.FromHours(dailyWorkHours);
            WorkPlaceType = workPlaceType;
            WorkDayType = WorkDayType.NORMAL;
        }
        #endregion

        #region Setters
        internal void SetStartTime(int startHour, int startMinute)
        {
            StartTime = Converter.ConvertHoursAndMinutesToTimeSpan(startHour, startMinute);
        }

        internal void SetStartTime(int? startHour, int? startMinute)
        {
            if (startHour != null && startMinute != null)
            {
                StartTime = Converter.ConvertHoursAndMinutesToTimeSpan((int)startHour, (int)startMinute);
            }
            else
            {
                if (startHour == null)
                {
                    throw new ArgumentNullException(nameof(startHour));
                }
                else
                {
                    throw new ArgumentNullException(nameof(startMinute));
                }
            }
        }

        internal void SetEndTime(int endHour, int endMinute)
        {
            EndTime = Converter.ConvertHoursAndMinutesToTimeSpan(endHour, endMinute);
        }

        internal void SetEndTime(int? endHour, int? endMinute)
        {
            if (endHour != null && endMinute != null)
            {
                EndTime = Converter.ConvertHoursAndMinutesToTimeSpan((int)endHour, (int)endMinute);
            }
            else
            {
                if (endHour == null)
                {
                    throw new ArgumentNullException(nameof(endHour));
                }
                else
                {
                    throw new ArgumentNullException(nameof(endMinute));
                }
            }
        }

        public void SetLunchBreakDuration(int? lunchBreakDuration)
        {
            if (lunchBreakDuration != null)
            {
                LunchBreakDuration = (int)lunchBreakDuration;
            }
            else
            {
                throw new ArgumentNullException(nameof(lunchBreakDuration));
            }
        }

        public void SetOtherBreakDuration(int? otherBreakDuration)
        {
            if (otherBreakDuration != null)
            {
                OtherBreakDuration = (int)otherBreakDuration;
            }
            else
            {
                throw new ArgumentNullException(nameof(otherBreakDuration));
            }
        }

        public void SetOverWorkDuration(int? overWorkDuration)
        {
            if (overWorkDuration != null)
            {
                OverWorkDuration = (int)overWorkDuration;
            }
            else
            {
                throw new ArgumentNullException(nameof(overWorkDuration));
            }
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
