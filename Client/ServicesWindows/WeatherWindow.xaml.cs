using Newtonsoft.Json;
using Server.DataParsing.DataObjects.Shares;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using Server.DataParsing.DataObjects.Weather;

namespace Client
{
    /// <summary>
    /// Interaction logic for WeatherWindow.xaml
    /// </summary>
    public partial class WeatherWindow : Window
    {
        private WhetherForecast _weatherData;
        private ClientPipe _client;

        public WeatherWindow(ClientPipe client)
        {
            InitializeComponent();
            _client = client;
            _client.OnWeatherRecived += _client_ServerResponseReceived;
            Closing += WeatherWindow_Closing;
        }

        private void WeatherWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _client.UnSubscribeToWeather();
            _client.OnWeatherRecived -= _client_ServerResponseReceived;
        }

        private void _client_ServerResponseReceived(object? sender, string e)
        {
            try
            {
                var weatherData = JsonConvert.DeserializeObject<WhetherForecast>(e);

                if (weatherData != null)
                {
                    _weatherData = weatherData;
                }
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"Error deserializing JSON: {ex.Message}");
                // Handle the error as needed
            }
        }

        private void Unsubscribe_Click(object sender, RoutedEventArgs e) => Close();
    }
}
