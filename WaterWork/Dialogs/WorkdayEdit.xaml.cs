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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WaterWork.Model;

namespace WaterWork.Dialogs
{
    /// <summary>
    /// Interaction logic for WorkdayEdit.xaml
    /// </summary>
    public partial class WorkdayEdit : Window, INotifyPropertyChanged
    {
        private WorkDay today;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int LunchBreakDuration { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal WorkdayEdit(WorkDay today)
        {
            InitializeComponent();
            this.today = today;

            StartTime = DateTime.Now.Date + today.StartTime;
            EndTime = DateTime.Now.Date + today.EndTime;
            LunchBreakDuration = today.LunchBreakDuration;

            editGrid.DataContext = this;
        }

        internal new WorkDay ShowDialog()
        {
            base.ShowDialog();

            today.StartTime = StartTime - DateTime.Now.Date;
            today.EndTime = EndTime - DateTime.Now.Date;
            today.LunchBreakDuration = LunchBreakDuration;

            return today;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
