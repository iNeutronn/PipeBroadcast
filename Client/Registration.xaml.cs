using Server.DataParsing.DataObjects.Shares;
using Server.DataParsing.DataObjects.Weather;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;

namespace Client
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {

        public Registration()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow mainWindow = new MainWindow(IPAddress.Parse(hostIp.Text));
                mainWindow.Show();
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid IP Address");
            }
        }
    }
}
