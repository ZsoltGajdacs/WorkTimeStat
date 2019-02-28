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
using WaterWork.Models;
using WaterWork.Services;

namespace WaterWork.Windows
{
    /// <summary>
    /// Interaction logic for StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {

        internal StatisticsWindow(WorkYear thisYear)
        {
            InitializeComponent();
            mainGrid.DataContext = this;

            double yWorkedHours = StatisticsService.GetYearlyWorkedHours(ref thisYear);
            double yFullHours = StatisticsService.GetYearlyTotalHours(ref thisYear);

            double mWorkedHours = StatisticsService.GetMonthlyWorkedHours(thisYear.GetCurrentMonth());
            double mFullHours = StatisticsService.GetMonthlyTotalHours(thisYear.GetCurrentMonth());

            yearlyWorkedHours.Content = yWorkedHours;
            yearlyFullHours.Content = yFullHours;
            yearlyLeftHours.Content = yWorkedHours - yFullHours;

            monthlyWorkedHours.Content = mWorkedHours;
            monthlyFullHours.Content = mFullHours;
            monthlyLeftHours.Content = mWorkedHours - mFullHours;
        }

    }
}
