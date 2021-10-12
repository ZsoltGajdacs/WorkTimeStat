using Newtonsoft.Json;
using System;
using WorkTimeStat.Enums;
using WorkTimeStat.Helpers;
using ZsGUtils.UIHelpers;

namespace WorkTimeStat.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    internal class WorkDay : BindableClass
    {
        private DateTime dayDate;
        private TimeSpan startTime;
        private TimeSpan endTime;
        private TimeSpan usageTime;
        private TimeSpan totalDailyUsage;
        private int lunchBreakDuration;
        private int otherBreakDuration;
        private int overWorkDuration;
        private WorkDayType workDayType;
        private WorkPlaceType workPlaceType;
        private bool isLunchTimeWorkTime;

        public DateTime DayDate { get => dayDate; set => SetAndNotifyPropertyChanged(ref dayDate, value); }
        public TimeSpan StartTime { get => startTime; set => SetAndNotifyPropertyChanged(ref startTime, value); }
        public TimeSpan EndTime { get => endTime; set => SetAndNotifyPropertyChanged(ref endTime, value); }
        public TimeSpan UsageTime { get => usageTime; set => SetAndNotifyPropertyChanged(ref usageTime, value); }
        public TimeSpan TotalDailyUsage { get => totalDailyUsage; set => SetAndNotifyPropertyChanged(ref totalDailyUsage, value); }
        public int LunchBreakDuration { get => lunchBreakDuration; set => SetAndNotifyPropertyChanged(ref lunchBreakDuration, value); }
        public int OtherBreakDuration { get => otherBreakDuration; set => SetAndNotifyPropertyChanged(ref otherBreakDuration, value); }
        public int OverWorkDuration { get => overWorkDuration; set => SetAndNotifyPropertyChanged(ref overWorkDuration, value); }
        public WorkDayType WorkDayType { get => workDayType; set => SetAndNotifyPropertyChanged(ref workDayType, value); }
        public WorkPlaceType WorkPlaceType { get => workPlaceType; set => SetAndNotifyPropertyChanged(ref workPlaceType, value); }
        public bool IsLunchTimeWorkTime { get => isLunchTimeWorkTime; set => SetAndNotifyPropertyChanged(ref isLunchTimeWorkTime, value); }

        #region CTORS
        public WorkDay(bool isLunchTimeWorkTime, double dailyWorkHours, WorkPlaceType workPlaceType)
        {
            IsLunchTimeWorkTime = isLunchTimeWorkTime;
            DayDate = DateTime.Now.Date;
            StartTime = RoundUpTime(DateTime.Now, TimeSpan.FromMinutes(15)).TimeOfDay;
            EndTime = StartTime + TimeSpan.FromHours(dailyWorkHours);
            WorkPlaceType = workPlaceType;
            WorkDayType = WorkDayType.WEEKDAY;
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

        private static DateTime RoundUpTime(DateTime date, TimeSpan roundTime)
        {
            return new DateTime((date.Ticks + roundTime.Ticks - 1) / roundTime.Ticks * roundTime.Ticks, date.Kind);
        }
    }
}
