using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WaterWork.Helpers;
using WaterWork.Models;

namespace WaterWork.Dialogs
{
    /// <summary>
    /// Interaction logic for WorkdayEdit.xaml
    /// </summary>
    public partial class WorkdayEdit : Window, INotifyPropertyChanged
    {
        private WorkDay today;
        private DateTime dateToday;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int LunchBreakDuration { get; set; }
        public int OtherBreakDuration { get; set; }
        public decimal ConsumptionCount { get; set; }
        public decimal BottleSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal WorkdayEdit(WorkDay today)
        {
            InitializeComponent();

            SetWindowPos();

            this.today = today;
            dateToday = DateTime.Now.Date;

            StartTime = dateToday + today.StartTime;
            EndTime = dateToday + today.EndTime;
            LunchBreakDuration = today.LunchBreakDuration;
            OtherBreakDuration = today.OtherBreakDuration;
            ConsumptionCount = today.WaterConsumptionCount;
            BottleSize = today.AmountOfLitreInOneUnit;

            editGrid.DataContext = this;
        }

        internal new WorkDay ShowDialog()
        {
            base.ShowDialog();

            today.StartTime = StartTime - dateToday;
            today.EndTime = EndTime - dateToday;
            today.LunchBreakDuration = LunchBreakDuration;
            today.OtherBreakDuration = OtherBreakDuration;
            today.WaterConsumptionCount = ConsumptionCount;
            today.AmountOfLitreInOneUnit = BottleSize;

            return today;
        }

        private void SetWindowPos()
        {
            Point pos = NativeMethods.GetMousePosition();
            //System.Drawing.Rectangle resolution = Screen.PrimaryScreen.Bounds;

            Top = pos.Y - (Height + 100);
            Left = pos.X - (Width / 2) - 100;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
