using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WaterWork.Events;
using WaterWork.Models;
using WaterWork.Services;
using WaterWork.Storage;

namespace WaterWork.Controls
{
    public partial class WorkDayEditControl : UserControl, INotifyPropertyChanged
    {
        private WorkDay today;
        private readonly DateTime dateToday;

        private DateTime startTime;
        private int startTimeHour = 6;
        private int startTimeMinute;

        private DateTime endTime;
        private int endTimeHour = 12;
        private int endTimeMinute;

        private int lunchBreakDuration;
        private int otherBreakDuration;
        private int overWorkDuration;

        private decimal consumptionCount;
        private decimal bottleSize;

        #region Properties
        public DateTime StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                NotifyPropertyChanged();
            }
        }

        public int StartTimeHour
        {
            get => startTimeHour;
            set
            {
                startTimeHour = value;
                NotifyPropertyChanged();
            }
        }

        public int StartTimeMinute
        {
            get => startTimeMinute;
            set
            {
                startTimeMinute = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime EndTime
        {
            get => endTime;
            set
            {
                endTime = value;
                NotifyPropertyChanged();
            }
        }

        public int EndTimeHour
        {
            get => endTimeHour;
            set
            {
                endTimeHour = value;
                NotifyPropertyChanged();
            }
        }

        public int EndTimeMinute
        {
            get => endTimeMinute;
            set
            {
                endTimeMinute = value;
                NotifyPropertyChanged();
            }
        }

        public int LunchBreakDuration
        {
            get => lunchBreakDuration;
            set
            {
                lunchBreakDuration = value;
                NotifyPropertyChanged();
            }
        }

        public int OtherBreakDuration
        {
            get => otherBreakDuration;
            set
            {
                otherBreakDuration = value;
                NotifyPropertyChanged();
            }
        }

        public int OverWorkDuration
        {
            get => overWorkDuration;
            set
            {
                overWorkDuration = value;
                NotifyPropertyChanged();
            }
        }

        public decimal ConsumptionCount
        {
            get => consumptionCount;
            set
            {
                consumptionCount = value;
                NotifyPropertyChanged();
            }
        }

        public decimal BottleSize
        {
            get => bottleSize;
            set
            {
                bottleSize = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        internal event CloseTheBallonEventHandler CloseBallon;

        internal WorkDayEditControl(WorkDay today)
        {
            InitializeComponent();

            WorkKeeper keeper = WorkKeeper.Instance;
            this.today = today ?? new WorkDay(keeper.Settings.IsLunchTimeWorkTimeDefault,
                                                keeper.Settings.AmountOfLitreInOneUnit);
            dateToday = DateTime.Now.Date;

            InitValues();

            editGrid.DataContext = this;
        }

        /// <summary>
        /// Initializes the window's vars
        /// </summary>
        private void InitValues()
        {
            StartTime = dateToday + today.StartTime;
            startTimeHour = today.StartTime.Hours;
            startTimeMinute = today.StartTime.Minutes;

            EndTime = dateToday + today.EndTime;
            endTimeHour = today.EndTime.Hours;
            endTimeMinute = today.EndTime.Minutes;

            LunchBreakDuration = today.LunchBreakDuration;
            OtherBreakDuration = today.OtherBreakDuration;
            OverWorkDuration = today.OverWorkDuration;
            ConsumptionCount = today.WaterConsumptionCount;
            BottleSize = today.AmountOfLitreInOneUnit;
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
            today.WaterConsumptionCount = ConsumptionCount;
            today.AmountOfLitreInOneUnit = BottleSize;

            WorkDayService.SetCurrentDay(ref today);
            StatisticsService.FullReCountWorkedDays();
            SaveService.SaveData(SaveUsage.No);
        }

        private TimeSpan CalcStartTime()
        {
            //return StartTime - dateToday;
            return TimeSpan.FromHours(startTimeHour) + TimeSpan.FromMinutes(startTimeMinute);
        }

        private TimeSpan CalcEndTime()
        {
            //return EndTime - dateToday;
            return TimeSpan.FromHours(EndTimeHour) + TimeSpan.FromMinutes(endTimeMinute);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseBallon?.Invoke();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveValues();
            CloseBallon?.Invoke();
        }
    }
}
