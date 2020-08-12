using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WaterWork.Helpers;
using WaterWork.Models;
using WaterWork.Storage;

namespace WaterWork.Controls
{
    public partial class WorkDayEditControl : UserControl, INotifyPropertyChanged
    {
        private readonly WorkDay today;
        private readonly DateTime dateToday;
        private readonly WorkKeeper keeper;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int LunchBreakDuration { get; set; }
        public int OtherBreakDuration { get; set; }
        public int OverWorkDuration { get; set; }
        public decimal ConsumptionCount { get; set; }
        public decimal BottleSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal WorkDayEditControl(ref WorkKeeper keeper, WorkDay today)
        {
            InitializeComponent();

            this.keeper = keeper;
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
            EndTime = dateToday + today.EndTime;
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
            today.StartTime = StartTime - dateToday;
            today.EndTime = EndTime - dateToday;
            today.LunchBreakDuration = LunchBreakDuration;
            today.OtherBreakDuration = OtherBreakDuration;
            today.OverWorkDuration = OverWorkDuration;
            today.WaterConsumptionCount = ConsumptionCount;
            today.AmountOfLitreInOneUnit = BottleSize;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveValues();

        }
    }
}
