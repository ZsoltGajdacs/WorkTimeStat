using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WorkTimeStat.Enums;
using WorkTimeStat.Events;
using WorkTimeStat.Models;
using WorkTimeStat.Services;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Controls
{
    public partial class WorkDayEditControl : UserControl, INotifyPropertyChanged
    {
        private WorkDay today;
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

        public IEnumerable<string> WorkTypes { get; private set; }
        private WorkDayType chosenOverWorkType;

        #region Properties
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                NotifyPropertyChanged();
            }
        }

        public int StartTimeHour
        {
            get => _startTimeHour;
            set
            {
                _startTimeHour = value;
                NotifyPropertyChanged();
            }
        }

        public int StartTimeMinute
        {
            get => _startTimeMinute;
            set
            {
                _startTimeMinute = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value;
                NotifyPropertyChanged();
            }
        }

        public int EndTimeHour
        {
            get => _endTimeHour;
            set
            {
                _endTimeHour = value;
                NotifyPropertyChanged();
            }
        }

        public int EndTimeMinute
        {
            get => _endTimeMinute;
            set
            {
                _endTimeMinute = value;
                NotifyPropertyChanged();
            }
        }

        public int LunchBreakDuration
        {
            get => _lunchBreakDuration;
            set
            {
                _lunchBreakDuration = value;
                NotifyPropertyChanged();
            }
        }

        public int OtherBreakDuration
        {
            get => _otherBreakDuration;
            set
            {
                _otherBreakDuration = value;
                NotifyPropertyChanged();
            }
        }

        public int OverWorkDuration
        {
            get => _overWorkDuration;
            set
            {
                _overWorkDuration = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        internal event CloseTheBallonEventHandler CloseBallon;

        internal WorkDayEditControl(WorkDay today)
        {
            InitializeComponent();

            WorkSettings settings = WorkKeeper.Instance.Settings;
            this.today = today ?? new WorkDay(settings.IsLunchTimeWorkTimeDefault,
                                                settings.AmountOfLitreInOneUnit, settings.DailyWorkHours);
            dateToday = DateTime.Now.Date;
            
            InitValues();
            SetBindings();
        }

        /// <summary>
        /// Initializes the window's vars
        /// </summary>
        private void InitValues()
        {
            StartTime = dateToday + today.StartTime;
            _startTimeHour = today.StartTime.Hours;
            _startTimeMinute = today.StartTime.Minutes;

            EndTime = dateToday + today.EndTime;
            _endTimeHour = today.EndTime.Hours;
            _endTimeMinute = today.EndTime.Minutes;

            LunchBreakDuration = today.LunchBreakDuration;
            OtherBreakDuration = today.OtherBreakDuration;
            OverWorkDuration = today.OverWorkDuration;

            WorkTypes = FillWorkTypeList();
        }

        private void SetBindings()
        {
            editGrid.DataContext = this;
            WorkType.ItemsSource = WorkTypes;

            chosenOverWorkType = today.WorkDayType;
            WorkType.SelectedIndex = (int)chosenOverWorkType;
        }

        /// <summary>
        /// Saves the window's vars to the object that is passed back
        /// </summary>
        private void SaveValues()
        {
            today.SetStartTime(StartTimeHour, StartTimeMinute);
            today.SetEndTime(EndTimeHour, EndTimeMinute);
            today.LunchBreakDuration = LunchBreakDuration;
            today.OtherBreakDuration = OtherBreakDuration;
            today.OverWorkDuration = OverWorkDuration;
            today.WorkDayType = chosenOverWorkType;

            WorkDayService.SetCurrentDay(ref today);
            StatisticsService.FullReCountWorkedDays();
            SaveService.SaveData(SaveUsage.No);
        }

        // TODO: This is probably not the best place for this. Find a better one!
        private static List<string> FillWorkTypeList()
        {
            IEnumerable<WorkDayType> enumList = EnumUtil.GetValues<WorkDayType>();

            List<string> enumNames = new List<string>();
            IEnumerator<WorkDayType> overWorkEnumerator = enumList.GetEnumerator();
            while (overWorkEnumerator.MoveNext())
            {
                WorkDayType workType = overWorkEnumerator.Current;
                enumNames.Add(workType.GetDescription());
            }

            return enumNames;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Event Handlers
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseBallon?.Invoke();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveValues();
            CloseBallon?.Invoke();
        }

        private void WorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = WorkType.SelectedValue.ToString();
            EnumMatchResult<WorkDayType> result = EnumUtil.GetEnumForString<WorkDayType>(selection);

            if (result != null)
            {
                chosenOverWorkType = result.FoundEnum;
            }
        }
        #endregion
    }
}
