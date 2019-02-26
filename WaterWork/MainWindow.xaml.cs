using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WaterWork.Dialogs;
using WaterWork.Model;

namespace WaterWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Keeper keeper;

        public MainWindow()
        {
            InitializeComponent();
            keeper = Keeper.Instance;
        }

        private void SettingsItem_Click(object sender, RoutedEventArgs e)
        {
            WorkdayEdit dayEdit = new WorkdayEdit(keeper.GetToday());
            keeper.SetToday(dayEdit.ShowDialog());
        }

        private void StatisticsItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Serialize();
            Application.Current.Shutdown();
        }

        private void TaskbarIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*WorkdayEdit dayEdit = new WorkdayEdit();
            dayEdit.ShowDialog();*/
        }

        private void TaskbarIcon_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Serialize()
        {
            FileInfo file = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            string savePath = file.DirectoryName + "\\" + DateTime.Now.Year.ToString() + ".json";

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
    }
}
