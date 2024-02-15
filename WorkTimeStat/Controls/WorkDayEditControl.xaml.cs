using SolidShineUi;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using UsageWatcher.Enums;
using UsageWatcher.Enums.Utils;
using WorkTimeStat.Controls.ViewModels;
using WorkTimeStat.Enums;
using WorkTimeStat.Events;
using WorkTimeStat.Helpers;
using WorkTimeStat.Models;
using WorkTimeStat.Services;
using WorkTimeStat.Storage;
using ZsGUtils.UIHelpers;

namespace WorkTimeStat.Controls
{
    public partial class WorkDayEditControl : UserControl
    {
        private readonly WorkDayEditControlVM viewModel;

        private WorkDayType chosenOverWorkType;
        private WorkPlaceType chosenWorkPlace;

        internal event CloseTheBallonEventHandler CloseBallon;

        internal WorkDayEditControl(WorkDay today)
        {
            InitializeComponent();

            viewModel = new WorkDayEditControlVM(today);

            InitValues();
            SetBindings();
        }

        private void InitValues()
        {
            chosenOverWorkType = viewModel.today.WorkDayType;
            WorkType.SelectedIndex = (int)chosenOverWorkType;

            chosenWorkPlace = viewModel.today.WorkPlaceType;
            WorkPlace.SelectedIndex = (int)chosenWorkPlace;
        }

        private void SetBindings()
        {
            editGrid.DataContext = viewModel;

            WorkType.ItemsSource = EnumUtil.GetValues<WorkDayType>();
            WorkPlace.ItemsSource = EnumUtil.GetValues<WorkPlaceType>();
        }

        /// <summary>
        /// Saves the window's vars to the object that is passed back
        /// </summary>
        private void SaveValues()
        {
            viewModel.SaveValues(chosenOverWorkType, chosenWorkPlace);

            WorkDayService.SetCurrentDay(ref viewModel.today);
            StatisticsService.FullReCountWorkedDays();
            SaveService.SaveData(SaveUsage.No);
        }

        #region Event Handlers
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            CloseBallon?.Invoke();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveValues();
            CloseBallon?.Invoke();
        }

        private void WorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedEnum = WorkType.SelectedValue;
            string enumName = string.Empty;
            if (selectedEnum is WorkDayType typeEnum)
            {
                enumName = typeEnum.GetDisplayName() ?? typeEnum.ToString();
            }

            EnumMatchResult<WorkDayType> result = EnumUtil.GetEnumForString<WorkDayType>(enumName);

            if (result != null)
            {
                chosenOverWorkType = result.FoundEnum;


                ShouldEnableOverWorkControl(result.FoundEnum != WorkDayType.OVERWORK_DAY);
            }
        }

        private void WorkPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedEnum = WorkPlace.SelectedValue;
            string enumName = string.Empty;
            if (selectedEnum is WorkPlaceType placeEnum)
            {
                enumName = placeEnum.GetDisplayName() ?? placeEnum.ToString();
            }

            EnumMatchResult<WorkPlaceType> result = EnumUtil.GetEnumForString<WorkPlaceType>(enumName);

            if (result != null)
            {
                chosenWorkPlace = result.FoundEnum;
            }
        }

        private void TimeCounters_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            viewModel.CalcAndSetTotalOfftime();
            viewModel.CalcAndSetTotalWorkTime();
        }
        #endregion

        private void ShouldEnableOverWorkControl(bool enableControl)
        {
            overWorkTime.IsEnabled = enableControl;
        }
    }
}
