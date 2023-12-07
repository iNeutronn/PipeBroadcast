using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPAddress _hostIP;
        private ClientPipe _clientPipe;
        public MainWindow(IPAddress hostIP)
        {
            InitializeComponent();
            _hostIP = hostIP;
            _clientPipe = new ClientPipe(_hostIP.ToString() == "0.0.0.0" ? "." : _hostIP.ToString());
            Task.Run(() => _clientPipe.Connect());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

        }

        private void WatherButton_Click(object sender, RoutedEventArgs e)
        {
            _clientPipe.SubscribeToWeather();
        }

        private void SharesButton_Click(object sender, RoutedEventArgs e)
        {
            _clientPipe.SubscribeToShares();
        }

        private void CurrencyButton_Click(object sender, RoutedEventArgs e)
        {
            _clientPipe.SubscribeToCurrency();
        }


    }
}
