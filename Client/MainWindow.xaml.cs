using System.Collections.Specialized;
using System.Diagnostics;
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
        WeatherWindow weatherWindow;
        SharesWindow sharesWindow;
        ExchangeWindow exchange;

        public MainWindow(IPAddress hostIP)
        {
            InitializeComponent();
            _hostIP = hostIP;
            _clientPipe = new ClientPipe(_hostIP.ToString() == "0.0.0.0" ? "." : _hostIP.ToString());
            _clientPipe.Connect();
            _clientPipe.OnExceptionRecived += _clientPipe_OnExceptionRecived;
            Closing += MainWindow_Closing;
        }

        private void _clientPipe_OnExceptionRecived(object? sender, string e)
        {
            Debug.WriteLine(e);

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                NoConnectionWindow noConnectionWindow = new NoConnectionWindow();

                noConnectionWindow.Show();

                // I want to close all other windowa while closing Main window but I have infinite waiting

                Close();
            });
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {


            _clientPipe.Dispose();
        }

        private void WatherButton_Click(object sender, RoutedEventArgs e)
        {
            if (_clientPipe.IsSubscribedToWeather) return;

            _clientPipe.SubscribeToWeather();

            weatherWindow = new(_clientPipe);

            weatherWindow.Show();
        }

        private void SharesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_clientPipe.IsSubscribedToShares) return;

            _clientPipe.SubscribeToShares();

            sharesWindow = new(_clientPipe);

            sharesWindow.Show();
        }

        private void CurrencyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_clientPipe.IsSubscribedToCurrency) return;

            _clientPipe.SubscribeToCurrency();

            exchange = new(_clientPipe);

            exchange.Show();
        }

    }
}
