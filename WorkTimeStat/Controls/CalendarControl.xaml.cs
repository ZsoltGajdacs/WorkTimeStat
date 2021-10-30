using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UsageWatcher.Enums;
using UsageWatcher.Enums.Utils;
using WorkTimeStat.Enums;
using WorkTimeStat.Events;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Services;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Controls
{
    public partial class CalendarControl : UserControl
    {
        private readonly WorkKeeper keeper;
        private readonly DateTime currDate;
        private readonly LocalizationHelper langHelper;

        private WorkDay chosenDay;
        private int numOfLeavesLeft;
        private bool leaveAutochk;
        private bool sickAutochk;
        private DateTime selectedDate;

        private WorkDayType editedWorkDayType;
        private WorkPlaceType editedWorkPlaceType;

        internal event CloseTheBallonEventHandler CloseBallon;

        public CalendarControl()
        {
            InitializeComponent();
            keeper = WorkKeeper.Instance;
            langHelper = LocalizationHelper.Instance;

            SetBindings();

            currDate = DateTime.Now.Date;

            SetToday();
            UpdateLeaveDays();
        }

        private void SetBindings()
        {
            mainGrid.DataContext = this;
            EditWorkType.ItemsSource = Enum.GetValues(typeof(WorkDayType)).Cast<WorkDayType>();
            EditWorkPlace.ItemsSource = Enum.GetValues(typeof(WorkPlaceType)).Cast<WorkPlaceType>();
        }

        private void UpdateLeaveDays()
        {
            numOfLeavesLeft = keeper.Settings.YearlyLeaveNumber - keeper.LeaveDays.Count;
            leaveDayNum.Content = numOfLeavesLeft + " / " + keeper.Settings.YearlyLeaveNumber;
        }

        private void SetToday()
        {
            chosenDay = WorkDayService.GetCurrentDay();
            if (chosenDay != null)
            {
                SetLabels(ref chosenDay);
                selectedDate = currDate.Date;
            }
            else
            {
                SetEmptyLabels();
            }

            chosenDateLabel.Content = DateTime.Today.ToLongDateString();
        }

        private void MainCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainCalendar.SelectedDate.HasValue)
            {
                selectedDate = mainCalendar.SelectedDate.Value.Date;
                chosenDateLabel.Content = selectedDate.ToLongDateString();

                CalendarSetLeaveDay(selectedDate);
                CalendarSetSickDay(selectedDate);

                chosenDay = WorkDayService.GetDayAtDate(selectedDate);

                RefreshLabels(ref chosenDay);
            }
        }

        private void RefreshLabels(ref WorkDay day)
        {
            if (day != null)
            {
                SetLabels(ref day);
            }
            else
            {
                SetEmptyLabels();
            }
        }

        private void SetLabels(ref WorkDay workDay)
        {
            string minute = langHelper.GetStringForKey("u_minute");
            
            startTimeValue.Content = workDay.StartTime;
            endTimeValue.Content = workDay.EndTime;
            lunchBreakTimeValue.Content = workDay.LunchBreakDuration + " " + minute;
            otherBreakTimeValue.Content = workDay.OtherBreakDuration + " " + minute;
            overWorkTimeValue.Content = workDay.OverWorkDuration + " " + minute;
            WorkTypeValue.Content = langHelper.GetStringForKey(workDay.WorkDayType.GetDisplayName());
            WorkPlaceValue.Content = langHelper.GetStringForKey(workDay.WorkPlaceType.GetDisplayName());
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
            WorkTypeValue.Content = NumberFormatter.NO_DATA;
            WorkPlaceValue.Content = NumberFormatter.NO_DATA;
            workedTimeValue.Content = NumberFormatter.NO_DATA;
            watchedTimeValue.Content = NumberFormatter.NO_DATA;
        }

        private void CalendarSetLeaveDay(DateTime selectedDate)
        {
            leaveAutochk = true; //To know that this was auto checked and not the user did it

            if (keeper.LeaveDays.Contains(selectedDate))
            {
                leaveDayChkbox.IsChecked = true;
            }
            else
            {
                leaveDayChkbox.IsChecked = false;
            }
        }

        private void CalendarSetSickDay(DateTime selectedDate)
        {
            sickAutochk = true; //To know that this was auto checked and not the user did it

            if (keeper.SickDays.Contains(selectedDate))
            {
                sickDayChkbox.IsChecked = true;
            }
            else
            {
                sickDayChkbox.IsChecked = false;
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

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (chosenDay != null)
            {
                MessageBoxResult result = MessageBox.Show(langHelper.GetStringForKey("cal_delete_confirm_dialog_content"),
                       langHelper.GetStringForKey("cal_delete_confirm_dialog_header"),
                       MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    WorkDayService.DeleteDay(ref chosenDay);
                    chosenDay = null;
                    RefreshLabels(ref chosenDay);
                }
            }
            else
            {
                MessageBox.Show(langHelper.GetStringForKey("cal_error_no_data_for_day_content"),
                       langHelper.GetStringForKey("cal_error_no_data_for_day_header"),
                       MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (chosenDay != null)
            {
                FillEditDataForChosenDay();
                HideLabels();
                ShowEditControls();
            }
            else
            {
                MessageBox.Show(langHelper.GetStringForKey("cal_error_no_data_for_day_content"),
                       langHelper.GetStringForKey("cal_error_no_data_for_day_header"),
                       MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveDataForChosenDay();
                RefreshLabels(ref chosenDay);
                HideEditControls();
                ShowLabels();

                SaveService.SaveData(SaveUsage.No);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show(langHelper.GetStringForKey("cal_error_incomplete_state_at_save_content"),
                    langHelper.GetStringForKey("cal_error_incomplete_state_at_save_header"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            HideEditControls();
            ShowLabels();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseBallon?.Invoke();
        }
        #endregion

        #region Selection events
        private void WorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedEnum = EditWorkType.SelectedValue;
            string enumName = string.Empty;
            if (selectedEnum is WorkDayType typeEnum)
            {
                enumName = typeEnum.GetDisplayName() ?? typeEnum.ToString();
            }

            EnumMatchResult<WorkDayType> result = EnumUtil.GetEnumForString<WorkDayType>(enumName);

            if (result != null)
            {
                editedWorkDayType = result.FoundEnum;
            }
        }

        private void WorkPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedEnum = EditWorkPlace.SelectedValue;
            string enumName = string.Empty;
            if (selectedEnum is WorkPlaceType placeEnum)
            {
                enumName = placeEnum.GetDisplayName() ?? placeEnum.ToString();
            }

            EnumMatchResult<WorkPlaceType> result = EnumUtil.GetEnumForString<WorkPlaceType>(enumName);

            if (result != null)
            {
                editedWorkPlaceType = result.FoundEnum;
            }
        }
        #endregion

        #region Edit methods
        private void FillEditDataForChosenDay()
        {
            WorkStartHour.Value = chosenDay.StartTime.Hours;
            WorkStartMinute.Value = chosenDay.StartTime.Minutes;
            WorkEndHour.Value = chosenDay.EndTime.Hours;
            WorkEndMinute.Value = chosenDay.EndTime.Minutes;
            EditWorkLaunchTime.Value = chosenDay.LunchBreakDuration;
            EditWorkbreakTime.Value = chosenDay.OtherBreakDuration;
            EditNonworkTime.Value = chosenDay.OverWorkDuration;
            EditWorkType.SelectedValue = chosenDay.WorkDayType;
            EditWorkPlace.SelectedValue = chosenDay.WorkPlaceType;
        }

        private void SaveDataForChosenDay()
        {
            chosenDay.SetStartTime(WorkStartHour.Value, WorkStartMinute.Value);
            chosenDay.SetEndTime(WorkEndHour.Value, WorkEndMinute.Value);
            chosenDay.SetLunchBreakDuration(EditWorkLaunchTime.Value);
            chosenDay.SetOtherBreakDuration(EditWorkbreakTime.Value);
            chosenDay.SetOverWorkDuration(EditNonworkTime.Value);
            chosenDay.WorkDayType = editedWorkDayType;
            chosenDay.WorkPlaceType = editedWorkPlaceType;
        }

        private void ShowLabels()
        {
            startTimeValue.Visibility = Visibility.Visible;
            endTimeValue.Visibility = Visibility.Visible;
            lunchBreakTimeValue.Visibility = Visibility.Visible;
            otherBreakTimeValue.Visibility = Visibility.Visible;
            overWorkTimeValue.Visibility = Visibility.Visible;
            WorkTypeValue.Visibility = Visibility.Visible;
            WorkPlaceValue.Visibility = Visibility.Visible;
        }

        private void HideLabels()
        {
            startTimeValue.Visibility = Visibility.Collapsed;
            endTimeValue.Visibility = Visibility.Collapsed;
            lunchBreakTimeValue.Visibility = Visibility.Collapsed;
            otherBreakTimeValue.Visibility = Visibility.Collapsed;
            overWorkTimeValue.Visibility = Visibility.Collapsed;
            WorkTypeValue.Visibility = Visibility.Collapsed;
            WorkPlaceValue.Visibility = Visibility.Collapsed;
        }

        private void ShowEditControls()
        {
            EditBtn.Visibility = Visibility.Collapsed;
            EditPanel.Visibility = Visibility.Visible;
            WorkStartPanel.Visibility = Visibility.Visible;
            WorkEndPanel.Visibility = Visibility.Visible;
            EditWorkLaunchTime.Visibility = Visibility.Visible;
            EditWorkbreakTime.Visibility = Visibility.Visible;
            EditNonworkTime.Visibility = Visibility.Visible;
            EditWorkType.Visibility = Visibility.Visible;
            EditWorkPlace.Visibility = Visibility.Visible;
        }

        private void HideEditControls()
        {
            EditBtn.Visibility = Visibility.Visible;
            EditPanel.Visibility = Visibility.Collapsed;
            WorkStartPanel.Visibility = Visibility.Collapsed;
            WorkEndPanel.Visibility = Visibility.Collapsed;
            EditWorkLaunchTime.Visibility = Visibility.Collapsed;
            EditWorkbreakTime.Visibility = Visibility.Collapsed;
            EditNonworkTime.Visibility = Visibility.Collapsed;
            EditWorkType.Visibility = Visibility.Collapsed;
            EditWorkPlace.Visibility = Visibility.Collapsed;
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
