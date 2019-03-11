using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for CalendarWindow.xaml
    /// </summary>
    public partial class CalendarWindow : Window
    {
        private Keeper keeper;

        internal CalendarWindow(ref Keeper keeper)
        {
            InitializeComponent();

            this.keeper = keeper;
        }

        private void SetLabels(ref WorkDay workDay)
        {
            startTimeLabel.Content = workDay.StartTime;
            endTimeLabel.Content = workDay.EndTime;
            lunchBreakTimeLabel.Content = workDay.LunchBreakDuration + " perc";
            otherBreakTimeLabel.Content = workDay.OtherBreakDuration + " perc";
            workedTimeLabel.Content = StatisticsService.GetDailyWorkedHours(workDay);
        }

        private void MainCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainCalendar.SelectedDate.HasValue)
            {
                DateTime date = mainCalendar.SelectedDate.Value;

                WorkYear workYear = keeper.GetYear(StatisticsService.GetYearForDate(date));

                if (workYear != null)
                {
                    WorkMonth workMonth = workYear.GetMonth(StatisticsService.GetMonthForDate(date));

                    if (workMonth != null)
                    {
                        WorkDay workDay = workMonth.GetDay(StatisticsService.GetDayForDate(date));

                        if (workDay != null)
                        {
                            SetLabels(ref workDay);
                        }
                    }
                }
                
                
            }
        }
    }
}
