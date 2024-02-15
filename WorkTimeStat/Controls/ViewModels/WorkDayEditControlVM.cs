using System;
using System.Globalization;
using WorkTimeStat.Enums;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Storage;
using ZsGUtils.UIHelpers;

namespace WorkTimeStat.Controls.ViewModels
{
    internal class WorkDayEditControlVM : BindableClass
    {
        private LocalizationHelper locHelper = LocalizationHelper.Instance;

        internal WorkDay today;
        private readonly DateTime dateToday;

        private DateTime _startTime;
        private int _startTimeHour = 6;
        private int _startTimeMinute;

        private DateTime _endTime;
        private int _endTimeHour = 12;
        private int _endTimeMinute;

        private int _lunchBreakDuration;
        private int _otherBreakDuration;
        private int _overWorkDuration;

        private string _totalWorktime = string.Empty;
        private int totalOfftimeNum;
        private string _totalOfftime = string.Empty;

        private int _dailyWorkHours;
        private int _dailyWorkMinutes;

        public DateTime StartTime { get => _startTime; set => SetAndNotifyPropertyChanged(ref _startTime, value); }
        public int StartTimeHour { get => _startTimeHour; set => SetAndNotifyPropertyChanged(ref _startTimeHour, value); }
        public int StartTimeMinute { get => _startTimeMinute; set => SetAndNotifyPropertyChanged(ref _startTimeMinute, value); }

        public DateTime EndTime { get => _endTime; set => SetAndNotifyPropertyChanged(ref _endTime, value); }
        public int EndTimeHour { get => _endTimeHour; set => SetAndNotifyPropertyChanged(ref _endTimeHour, value); }
        public int EndTimeMinute { get => _endTimeMinute; set => SetAndNotifyPropertyChanged(ref _endTimeMinute, value); }

        public int LunchBreakDuration { get => _lunchBreakDuration; set => SetAndNotifyPropertyChanged(ref _lunchBreakDuration, value); }
        public int OtherBreakDuration { get => _otherBreakDuration; set => SetAndNotifyPropertyChanged(ref _otherBreakDuration, value); }
        public int OverWorkDuration { get => _overWorkDuration; set => SetAndNotifyPropertyChanged(ref _overWorkDuration, value); }

        public string TotalWorktime { get => _totalWorktime; set => SetAndNotifyPropertyChanged(ref _totalWorktime, value); }
        public string TotalOfftime { get => _totalOfftime; set => SetAndNotifyPropertyChanged(ref _totalOfftime, value); }
        public int DailyWorkHours { get => _dailyWorkHours; set => SetAndNotifyPropertyChanged(ref _dailyWorkHours, value); }
        public int DailyWorkMinutes { get => _dailyWorkMinutes; set => SetAndNotifyPropertyChanged(ref _dailyWorkMinutes, value); }

        public WorkDayEditControlVM(WorkDay today)
        {
            dateToday = DateTime.Now.Date;
            WorkSettings settings = WorkKeeper.Instance.Settings;
            this.today = today ?? new WorkDay(settings.IsLunchTimeWorkTimeDefault, settings.GetDefaultHours(), settings.WorkPlaceType);

            InitValues();
        }

        private void InitValues()
        {
            StartTime = dateToday + today.StartTime;
            _startTimeHour = today.StartTime.Hours;
            _startTimeMinute = today.StartTime.Minutes;

            EndTime = dateToday + today.EndTime;
            _endTimeHour = today.EndTime.Hours;
            _endTimeMinute = today.EndTime.Minutes;

            _dailyWorkHours = today.RequiredWorkLength.Hours;
            _dailyWorkMinutes = today.RequiredWorkLength.Minutes;

            LunchBreakDuration = today.LunchBreakDuration;
            OtherBreakDuration = today.OtherBreakDuration;
            OverWorkDuration = today.OverWorkDuration;

            CalcAndSetTotalOfftime();
            CalcAndSetTotalWorkTime();
        }

        internal void CalcAndSetTotalWorkTime()
        {
            TimeSpan startTime = Converter.ConvertHoursAndMinutesToTimeSpan(_startTimeHour, _startTimeMinute);
            TimeSpan endTime = Converter.ConvertHoursAndMinutesToTimeSpan(_endTimeHour, _endTimeMinute);
            double total = ((endTime - startTime) - TimeSpan.FromMinutes(totalOfftimeNum)).TotalHours;

            TotalWorktime = string.Format(CultureInfo.CurrentCulture, "{0:0.00} {1}", total, locHelper.GetStringForKey("u_hour"));
        }

        internal void CalcAndSetTotalOfftime()
        {
            totalOfftimeNum = LunchBreakDuration + OtherBreakDuration;
            TotalOfftime = string.Format(CultureInfo.CurrentCulture, "{0} {1}", totalOfftimeNum, locHelper.GetStringForKey("u_minute"));
        }

        internal void SaveValues(WorkDayType chosenWorkType, WorkPlaceType chosenWorkPlaceType)
        {
            today.SetStartTime(StartTimeHour, StartTimeMinute);
            today.SetEndTime(EndTimeHour, EndTimeMinute);
            today.LunchBreakDuration = LunchBreakDuration;
            today.OtherBreakDuration = OtherBreakDuration;
            today.OverWorkDuration = OverWorkDuration;
            today.WorkDayType = chosenWorkType;
            today.WorkPlaceType = chosenWorkPlaceType;
        }
    }
}
