using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using UsageWatcher.Enums;
using UsageWatcher.Enums.Utils;
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

        private WorkDayType chosenOverWorkType;
        private WorkPlaceType chosenWorkPlace;

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
                                                settings.AmountOfLitreInOneUnit, settings.DailyWorkHours, settings.WorkPlaceType);
            dateToday = DateTime.Now.Date;
            
            InitValues();
            SetBindings();
        }

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

            chosenOverWorkType = today.WorkDayType;
            WorkType.SelectedIndex = (int)chosenOverWorkType;

            chosenWorkPlace = today.WorkPlaceType;
            WorkPlace.SelectedIndex = (int)chosenWorkPlace;
        }

        private void SetBindings()
        {
            editGrid.DataContext = this;
            
            WorkType.ItemsSource = Enum.GetValues(typeof(WorkDayType)).Cast<WorkDayType>();
            WorkPlace.ItemsSource = Enum.GetValues(typeof(WorkPlaceType)).Cast<WorkPlaceType>();
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
            today.WorkPlaceType = chosenWorkPlace;

            WorkDayService.SetCurrentDay(ref today);
            StatisticsService.FullReCountWorkedDays();
            SaveService.SaveData(SaveUsage.No);
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
            object selectedEnum = WorkType.SelectedValue;
            string enumName = string.Empty;
            if (selectedEnum is WorkDayType typeEnum)
            {
                enumName = typeEnum.GetDisplayName() ?? typeEnum.ToString();
            }

            EnumMatchResult<WorkDayType> result = EnumUtil.GetEnumForString<WorkDayType>(enumName);

            if (result != null)
            {
                chosenOverWorkType = result.FoundEnum;
            }
        }

        private void WorkPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedEnum = WorkPlace.SelectedValue;
            string enumName = string.Empty;
            if (selectedEnum is WorkPlaceType placeEnum)
            {
                enumName = placeEnum.GetDisplayName() ?? placeEnum.ToString();
            }

            EnumMatchResult<WorkPlaceType> result = EnumUtil.GetEnumForString<WorkPlaceType>(enumName);

            if (result != null)
            {
                chosenWorkPlace = result.FoundEnum;
            }
        }
        #endregion
    }
}
