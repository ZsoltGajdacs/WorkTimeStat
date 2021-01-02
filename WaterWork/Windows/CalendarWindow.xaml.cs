using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WaterWork.Helpers;
using WaterWork.Models;
using WaterWork.Services;
using WaterWork.Storage;

namespace WaterWork.Windows
{
    public partial class CalendarWindow : Window
    {
        private const string EDIT_BTN_EDIT_LABEL = "Javítás";
        private const string EDIT_BTN_SAVE_LABEL = "Mentés";

        private readonly WorkKeeper keeper;
        private readonly DateTime currDate;
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
            startTimeValue.Content = workDay.StartTime;
            endTimeValue.Content = workDay.EndTime;
            lunchBreakTimeValue.Content = workDay.LunchBreakDuration + " perc";
            otherBreakTimeValue.Content = workDay.OtherBreakDuration + " perc";
            overWorkTimeValue.Content = workDay.OverWorkDuration + " perc";
            workedTimeValue.Content = StatisticsService.CalcDailyWorkedHours(workDay);

            double daysUsage = StatisticsService.GetUsageForDay(workDay);
            watchedTimeValue.Content = NumberFormatter.FormatNum(daysUsage);
        }

        private void SetEmptyLabels()
        {
            startTimeValue.Content = NumberFormatter.NO_DATA;
            endTimeValue.Content = NumberFormatter.NO_DATA;
            lunchBreakTimeValue.Content = NumberFormatter.NO_DATA;
            otherBreakTimeValue.Content = NumberFormatter.NO_DATA;
            overWorkTimeValue.Content = NumberFormatter.NO_DATA;
            workedTimeValue.Content = NumberFormatter.NO_DATA;
            watchedTimeValue.Content = NumberFormatter.NO_DATA;
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

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EditPanel.Visibility == Visibility.Collapsed)
            {
                EditBtn.Visibility = Visibility.Collapsed;
                EditPanel.Visibility = Visibility.Visible;
            }


        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {

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
