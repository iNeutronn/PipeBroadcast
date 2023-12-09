using System.Net;
using System.Windows;


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

            WeatherWindow weatherWindow = new (_clientPipe);

            weatherWindow.Show();
        }

        private void SharesButton_Click(object sender, RoutedEventArgs e)
        {
            if(_clientPipe.IsSubscribedToShares) return;

            _clientPipe.SubscribeToShares();

            SharesWindow sharesWindow = new (_clientPipe);

            sharesWindow.Show();
        }

        private void CurrencyButton_Click(object sender, RoutedEventArgs e)
        {
            if(_clientPipe.IsSubscribedToCurrency) return;

            _clientPipe.SubscribeToCurrency();

            ExchangeWindow exchange = new (_clientPipe);

            exchange.Show();
        }

    }
}
