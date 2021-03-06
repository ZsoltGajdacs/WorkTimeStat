using System;
using System.Linq;
using System.Windows.Controls;
using UsageWatcher.Enums;
using WorkTimeStat.Enums;
using WorkTimeStat.Events;
using WorkTimeStat.Storage;

namespace WorkTimeStat.Controls
{
    public partial class SettingsControl : UserControl
    {
        private readonly WorkKeeper keeper;

        internal event CloseTheBallonEventHandler CloseBallon;

        public SettingsControl()
        {
            InitializeComponent();
            keeper = WorkKeeper.Instance;

            ResCombo.ItemsSource = Enum.GetValues(typeof(Resolution)).Cast<Resolution>();
            WorkPlaceCombo.ItemsSource = Enum.GetValues(typeof(WorkPlaceType)).Cast<WorkPlaceType>();
            LangCombo.ItemsSource = Enum.GetValues(typeof(AvailableLanguages)).Cast<AvailableLanguages>();

            mainGrid.DataContext = keeper.Settings;
        }

        private void SaveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CloseBallon?.Invoke();
        }
    }
}
