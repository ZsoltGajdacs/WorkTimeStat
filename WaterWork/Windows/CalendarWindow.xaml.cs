using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WaterWork.Models;
using WaterWork.Services;
using WaterWork.Storage;

namespace WaterWork.Windows
{
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

            SetToday();
            UpdateLeaveDays();
        }

        private void UpdateLeaveDays()
        {
            numOfLeavesLeft = keeper.Settings.YearlyLeaveNumber - keeper.LeaveDays.Count;
            leaveDayNum.Content = numOfLeavesLeft + " / " + keeper.Settings.YearlyLeaveNumber;
        }

        private void SetToday()
        {
            WorkDay workDay = WorkDayService.GetCurrentDay();
            if (workDay != null)
            {
                SetLabels(ref workDay);
                selectedDate = currDate.Date;
                chosenDateLabel.Content = selectedDate.ToLongDateString();
            }
            else
            {
                SetEmptyLabels();
            }
        }

        private void MainCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainCalendar.SelectedDate.HasValue)
            {
                selectedDate = mainCalendar.SelectedDate.Value.Date;
                chosenDateLabel.Content = selectedDate.ToLongDateString();

                CalendarSetLeaveDay(selectedDate);
                CalendarSetSickDay(selectedDate);

                WorkDay workDay = WorkDayService.GetDayAtDate(selectedDate);

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

        private void SetLabels(ref WorkDay workDay)
        {
            startTimeLabel.Content = workDay.StartTime;
            endTimeLabel.Content = workDay.EndTime;
            lunchBreakTimeLabel.Content = workDay.LunchBreakDuration + " perc";
            otherBreakTimeLabel.Content = workDay.OtherBreakDuration + " perc";
            overWorkTimeLabel.Content = workDay.OverWorkDuration + " perc";
            workedTimeLabel.Content = StatisticsService.GetDailyWorkedHours(workDay);

            double daysUsage = StatisticsService.GetUsageForDay(ref workDay);
            watchedTimeLabel.Content = daysUsage != 0 ? daysUsage.ToString() : NO_DATA;
        }

        private void SetEmptyLabels()
        {
            startTimeLabel.Content = NO_DATA;
            endTimeLabel.Content = NO_DATA;
            lunchBreakTimeLabel.Content = NO_DATA;
            otherBreakTimeLabel.Content = NO_DATA;
            overWorkTimeLabel.Content = NO_DATA;
            workedTimeLabel.Content = NO_DATA;
            watchedTimeLabel.Content = NO_DATA;
        }

        private void CalendarSetLeaveDay(DateTime selectedDate)
        {
            leaveAutochk = true; //To know that this was auto checked and not the user did it

            if (keeper.LeaveDays.Contains(selectedDate))
            {
                leaveDayChkbox.IsChecked = true;
                leaveDayChkbox.IsEnabled = selectedDate >= currDate;
            }
            else if (selectedDate < currDate)
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

        private void CalendarSetSickDay(DateTime selectedDate)
        {
            sickAutochk = true; //To know that this was auto checked and not the user did it

            if (keeper.SickDays.Contains(selectedDate))
            {
                sickDayChkbox.IsChecked = true;
                sickDayChkbox.IsEnabled = selectedDate >= currDate;
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

        #region Click events
        private void LeaveDayChkbox_Click(object sender, RoutedEventArgs e)
        {
            if (numOfLeavesLeft > 0 && !keeper.LeaveDays.Contains(selectedDate))
            {
                keeper.LeaveDays.Add(selectedDate);

                // Ha szabin vagyok nem lehetek betegen!
                sickDayChkbox.IsChecked = false;
                keeper.SickDays.Remove(selectedDate);
            }
            else if (keeper.LeaveDays.Contains(selectedDate) && !leaveAutochk)
            {
                keeper.LeaveDays.Remove(selectedDate);
            }

            UpdateLeaveDays();
        }

        private void SickDayChkbox_Click(object sender, RoutedEventArgs e)
        {
            if (!keeper.SickDays.Contains(selectedDate))
            {
                keeper.SickDays.Add(selectedDate);

                // Ha betegre megyek nem lehetek szabin....
                leaveDayChkbox.IsChecked = false;
                keeper.LeaveDays.Remove(selectedDate);
                UpdateLeaveDays();
            }
            else if (keeper.SickDays.Contains(selectedDate) && !sickAutochk)
            {
                keeper.SickDays.Remove(selectedDate);
            }
        }

        #endregion

        #region Fixes

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

        /// <summary>
        /// Enélkül a Calendarra kattintva kétszer kell máshová kattintani, hogy vegye a lapot!
        /// https://stackoverflow.com/questions/5543119/wpf-button-takes-two-clicks-to-fire-click-event#6420914
        /// </summary>
        private void Calendar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            Mouse.Capture(null);
        }

        #endregion
    }
}
