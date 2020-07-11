using System.Windows;
using WaterWork.Storage;

namespace WaterWork.Windows
{
    public partial class SettingsWindow : Window
    {
        private WorkKeeper keeper;

        internal SettingsWindow(ref WorkKeeper keeper)
        {
            InitializeComponent();
            this.keeper = keeper;

            mainGrid.DataContext = keeper.Settings;
        }
    }
}
