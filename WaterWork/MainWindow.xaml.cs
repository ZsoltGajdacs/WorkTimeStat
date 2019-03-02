using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WaterWork.Dialogs;
using WaterWork.Models;
using WaterWork.Windows;

namespace WaterWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Keeper keeper;
        private readonly string savePath;

        public MainWindow()
        {
            InitializeComponent();
            FileInfo file = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            savePath = file.DirectoryName + "\\waterwork - " + DateTime.Now.Year.ToString() + ".json";

            keeper = Deserialize();

            if (keeper == null)
                keeper = new Keeper();
            else
                keeper.GetCurrentYear().CountWorkedDays();
        }

        #region Window Events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Serialize();
        }
        #endregion

        #region Tray Click Events
        private void TaskbarIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            WorkdayEdit dayEdit = new WorkdayEdit(keeper.GetCurrentDay());
            WorkDay today = dayEdit.ShowDialog();

            keeper.SetCurrentDay(ref today);
        }

        private void TaskbarIcon_TrayRightMouseUp(object sender, RoutedEventArgs e)
        {
            WorkDay today = keeper.GetCurrentDay();
            today.IncreaseWaterConsumption();

            decimal waterAmount = today.WaterConsumptionCount * today.AmountOfLitreInOneUnit;

            taskbarIcon.ShowBalloonTip("Vízfogyasztás", "Már " + waterAmount + "l vizet ittál ma!", 
                                            Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);
            Thread.Sleep(3000);
            taskbarIcon.HideBalloonTip();
        }
        #endregion

        #region Menu Click Events
        private void SettingsItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void StatisticsItem_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindow statisticsWindow = new StatisticsWindow(keeper.GetCurrentYear());
            statisticsWindow.Show();
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region JSON serialization
        private void Serialize()
        {
            TextWriter writer = null;
            try
            {
                string output = JsonConvert.SerializeObject(keeper);
                writer = new StreamWriter(savePath, false);
                writer.Write(output);
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

        private Keeper Deserialize()
        {
            if (new FileInfo(savePath).Exists)
            {
                TextReader reader = null;
                try
                {
                    reader = new StreamReader(savePath);
                    var fileContents = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<Keeper>(fileContents);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
