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
            _clientPipe.Connect();
        }

        private void WatherButton_Click(object sender, RoutedEventArgs e)
        {
            if (_clientPipe.IsSubscribedToWeather) return;

            _clientPipe.SubscribeToWeather();

            WeatherWindow weatherWindow = new WeatherWindow(_clientPipe);

            weatherWindow.Show();
        }

        private void SharesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_clientPipe.IsSubscribedToShares) return;

            _clientPipe.SubscribeToShares();

            SharesWindow sharesWindow = new SharesWindow(_clientPipe);

            sharesWindow.Show();
        }

        private void CurrencyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_clientPipe.IsSubscribedToCurrency) return;

            _clientPipe.SubscribeToCurrency();

            ExchangeWindow exchange = new ExchangeWindow(_clientPipe);

            exchange.Show();
        }

    }
}
