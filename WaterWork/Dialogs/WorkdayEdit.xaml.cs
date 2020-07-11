using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using WaterWork.Helpers;
using WaterWork.Models;
using WaterWork.Storage;

namespace WaterWork.Dialogs
{
    public partial class WorkdayEdit : Window, INotifyPropertyChanged
    {
        private readonly WorkDay today;
        private readonly DateTime dateToday;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int LunchBreakDuration { get; set; }
        public int OtherBreakDuration { get; set; }
        public int OverWorkDuration { get; set; }
        public decimal ConsumptionCount { get; set; }
        public decimal BottleSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal WorkdayEdit(WorkSettings settings, WorkDay today)
        {
            InitializeComponent();

            this.today = today ?? new WorkDay(settings.IsLunchTimeWorkTimeDefault,
                                                settings.AmountOfLitreInOneUnit);
            dateToday = DateTime.Now.Date;

            InitValues();

            editGrid.DataContext = this;
        }

        internal new WorkDay ShowDialog()
        {
            SetWindowPos();

            base.ShowDialog();

            SaveValues();

            return today;
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

        private void SetWindowPos()
        {
            Point cursorPos = NativeMethods.GetMousePosition();
            //System.Drawing.Rectangle resolution = Screen.PrimaryScreen.Bounds;
            double scaling = DPI.GetScaling();
            double posY = cursorPos.Y;
            double posX = cursorPos.X;

            if (scaling < 1.15)
            {
                posY -= Height + 75;
                posX -= (Width / 2) + 50;
            }
            else
            {
                posY -= Height + 275;
                posX -= (Width / 2) + 425;
            }

            Top = posY;
            Left = posX;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
