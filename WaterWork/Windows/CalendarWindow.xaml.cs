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
        private static string NO_DATA = "-";

        private WorkKeeper keeper;
        private DateTime currDate;
        private int numOfLeavesLeft;
        private bool leaveAutochk;
        private bool sickAutochk;
        private DateTime selectedDate;

        internal CalendarWindow(ref WorkKeeper keeper)
        {
            InitializeComponent();

            mainGrid.DataContext = this;
            this.keeper = keeper;

            currDate = DateTime.Now.Date;

            // SetToday();
            UpdateLeaveDays();

            // Since it always starts at today, disable is right away
            leaveDayChkbox.IsEnabled = false;
        }

        private void UpdateLeaveDays()
        {
            numOfLeavesLeft = keeper.YearlyLeaveNumber - keeper.LeaveDays.Count;
            leaveDayNum.Content = numOfLeavesLeft;
        }

        /// <summary>
        /// INCOMPLETE! Sets the initial day
        /// Make it so it doesn't generate a new day!
        /// </summary>
        private void SetToday()
        {
            WorkDay workDay = keeper.GetCurrentDay();
            if (workDay != null)
            {
                SetLabels(ref workDay);
            }
        }

        private void SetLabels(ref WorkDay workDay)
        {
            startTimeLabel.Content = workDay.StartTime;
            endTimeLabel.Content = workDay.EndTime;
            lunchBreakTimeLabel.Content = workDay.LunchBreakDuration + " perc";
            otherBreakTimeLabel.Content = workDay.OtherBreakDuration + " perc";
            workedTimeLabel.Content = StatisticsService.GetDailyWorkedHours(workDay);
        }

        private void SetEmptyLabels()
        {
            startTimeLabel.Content = NO_DATA;
            endTimeLabel.Content = NO_DATA;
            lunchBreakTimeLabel.Content = NO_DATA;
            otherBreakTimeLabel.Content = NO_DATA;
            workedTimeLabel.Content = NO_DATA;
        }

        private void SetLeaveDay(DateTime selectedDate)
        {
            leaveAutochk = true; //To know that this was auto checked and not the user did it

            if (keeper.LeaveDays.Contains(selectedDate))
            {
                leaveDayChkbox.IsChecked = true;
                leaveDayChkbox.IsEnabled = false;
            }
            else if (selectedDate <= currDate)
            {
                leaveDayChkbox.IsChecked = false;
                leaveDayChkbox.IsEnabled = false;
            }
            else
            {
                leaveDayChkbox.IsChecked = false;
                leaveDayChkbox.IsEnabled = true;
            }
        }

        private void SetSickDay(DateTime selectedDate)
        {
            sickAutochk = true; //To know that this was auto checked and not the user did it

            if (keeper.SickDays.Contains(selectedDate))
            {
                sickDayChkbox.IsChecked = true;
                sickDayChkbox.IsEnabled = false;
            }
            else if (selectedDate < currDate)
            {
                sickDayChkbox.IsChecked = false;
                sickDayChkbox.IsEnabled = false;
            }
            else
            {
                sickDayChkbox.IsChecked = false;
                sickDayChkbox.IsEnabled = true;
            }
        }

        private void MainCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainCalendar.SelectedDate.HasValue)
            {
                selectedDate = mainCalendar.SelectedDate.Value.Date;
                chosenDateLabel.Content = selectedDate.ToLongDateString();

                SetLeaveDay(selectedDate);
                SetSickDay(selectedDate);

                WorkYear workYear = keeper.GetYear(StatisticsService.GetYearForDate(selectedDate));

                if (workYear != null)
                {
                    WorkMonth workMonth = workYear.GetMonth(StatisticsService.GetMonthForDate(selectedDate));

                    if (workMonth != null)
                    {
                        WorkDay workDay = workMonth.GetDay(StatisticsService.GetDayForDate(selectedDate));

                        if (workDay != null)
                        {
                            SetLabels(ref workDay);
                        }
                        else
                        {
                            SetEmptyLabels();
                        }
                    }
                }
            }
        }

        private void LeaveDayChkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (numOfLeavesLeft > 0 && !keeper.LeaveDays.Contains(selectedDate))
            {
                keeper.LeaveDays.Add(selectedDate);
            }
            else if (keeper.LeaveDays.Contains(selectedDate) && !leaveAutochk)
            {
                keeper.LeaveDays.Remove(selectedDate);
            }

            UpdateLeaveDays();
        }

        private void SickDayChkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (!keeper.SickDays.Contains(selectedDate))
            {
                keeper.SickDays.Add(selectedDate);
            }
            else if (keeper.SickDays.Contains(selectedDate) && !sickAutochk)
            {
                keeper.SickDays.Remove(selectedDate);
            }
        }

        /// <summary>
        /// So I know it was me and not the code (code checking generates event too)
        /// </summary>
        private void LeaveDayChkbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            leaveAutochk = false;
        }
        
        /// <summary>
        /// So I know it was me and not the code (code checking generates event too)
        /// </summary>
        private void SickDayChkbox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sickAutochk = false;
        }
    }
}
