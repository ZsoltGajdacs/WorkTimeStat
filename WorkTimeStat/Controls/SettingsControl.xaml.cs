using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UsageWatcher.Enums;
using WorkTimeStat.Enums;
using WorkTimeStat.Events;
using WorkTimeStat.Helpers;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Controls
{
    public partial class SettingsControl : UserControl
    {
        private readonly WorkKeeper keeper;
        private readonly LocalizationHelper locHelp;

        internal event CloseTheBallonEventHandler CloseBallon;

        public SettingsControl()
        {
            InitializeComponent();
            keeper = WorkKeeper.Instance;
            locHelp = LocalizationHelper.Instance;

            ResCombo.ItemsSource = Enum.GetValues(typeof(Resolution)).Cast<Resolution>();
            WorkPlaceCombo.ItemsSource = Enum.GetValues(typeof(WorkPlaceType)).Cast<WorkPlaceType>();
            LangCombo.ItemsSource = Enum.GetValues(typeof(AvailableLanguages)).Cast<AvailableLanguages>();

            mainGrid.DataContext = keeper.Settings;
        }

        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (keeper.Settings.DailyWorkHours == default || keeper.Settings.YearlyLeaveNumber == default)
            {
                MessageBox.Show(locHelp.GetStringForKey("set_error_incomplete_setup_content"),
                       locHelp.GetStringForKey("set_error_incomplete_setup_header"),
                       MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                CloseBallon?.Invoke();
            }
        }

        private void LangCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedEnum = LangCombo.SelectedValue;
            string langCode = string.Empty;
            if (selectedEnum is AvailableLanguages placeEnum)
            {
                langCode = placeEnum.GetDescription();
            }

            try
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCode);
            }
            catch (CultureNotFoundException)
            {
                // What can I do?
            }
            
        }
    }
}
