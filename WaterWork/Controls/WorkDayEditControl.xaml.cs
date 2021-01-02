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

        public IEnumerable<string> OverWorkTypes { get; private set; }
        private OverWorkType chosenOverWorkType;

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

            OverWorkTypes = FillOverWorkList();
        }

        private void SetBindings()
        {
            editGrid.DataContext = this;
            overWorkType.ItemsSource = OverWorkTypes;

            chosenOverWorkType = today.OverWorkType;
            overWorkType.SelectedIndex = (int)chosenOverWorkType;
        }

        /// <summary>
        /// Saves the window's vars to the object that is passed back
        /// </summary>
        private void SaveValues()
        {
            today.StartTime = CalcStartTime();
            today.EndTime = CalcEndTime();
            today.LunchBreakDuration = LunchBreakDuration;
            today.OtherBreakDuration = OtherBreakDuration;
            today.OverWorkDuration = OverWorkDuration;
            today.OverWorkType = chosenOverWorkType;

            WorkDayService.SetCurrentDay(ref today);
            StatisticsService.FullReCountWorkedDays();
            SaveService.SaveData(SaveUsage.No);
        }

        // TODO: This is probably not the best place for this. Find a better one!
        private static List<string> FillOverWorkList()
        {
            IEnumerable<OverWorkType> enumList = EnumUtil.GetValues<OverWorkType>();

            List<string> enumNevek = new List<string>();
            IEnumerator<OverWorkType> overWorkEnumerator = enumList.GetEnumerator();
            while (overWorkEnumerator.MoveNext())
            {
                OverWorkType workType = overWorkEnumerator.Current;
                enumNevek.Add(workType.GetDescription());
            }

            return enumNevek;
        }

        private TimeSpan CalcStartTime()
        {
            //return StartTime - dateToday;
            return TimeSpan.FromHours(_startTimeHour) + TimeSpan.FromMinutes(_startTimeMinute);
        }

        private TimeSpan CalcEndTime()
        {
            //return EndTime - dateToday;
            return TimeSpan.FromHours(EndTimeHour) + TimeSpan.FromMinutes(_endTimeMinute);
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

        private void OverWorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = overWorkType.SelectedValue.ToString();
            EnumMatchResult<OverWorkType> result = EnumUtil.GetEnumForString<OverWorkType>(selection);

            if (result != null)
            {
                chosenOverWorkType = result.FoundEnum;
            }
        }
        #endregion
    }
}
