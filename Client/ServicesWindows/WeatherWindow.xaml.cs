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
using System.Threading;
using System.IO;


namespace Client
{
    /// <summary>
    /// Interaction logic for WeatherWindow.xaml
    /// </summary>
    public partial class WeatherWindow : Window
    {
        private WhetherForecast _weatherData;
        private ClientPipe _client;
        private Dictionary<int, BitmapImage> icons = new();
        private int forecastNum = 0;
        bool isDay = true;

        public WeatherWindow(ClientPipe client)
        {
            InitializeComponent();
            _client = client;
            _client.OnWeatherRecived += _client_ServerResponseReceived;
            Closing += WeatherWindow_Closing;

            LoadIcons();

            RewriteInterfacePeriodically();
        }

        private void LoadIcons()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string iconsPath = System.IO.Path.Combine(basePath, "..", "..", "..", "WeatherIcons");

            var files = Directory.GetFiles(iconsPath);
            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(file);
                var image = new BitmapImage(new Uri(file));
                icons.Add(int.Parse(name), image);
            }
        }

        private void RewriteInterfacePeriodically()
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    RewriteInterface();

                    Thread.Sleep(200);
                }
            });
            t.Name = "RewriteInterface";
            t.IsBackground = true;
            t.Start();
        }

        private void RewriteInterface()
        {
            if (_weatherData == null) return;

            dynamic forecast = (isDay) ? _weatherData.DailyForecasts[forecastNum].Day : _weatherData.DailyForecasts[forecastNum].Night;

            var temperature = _weatherData.DailyForecasts[forecastNum].Temperature;
            var wind = forecast.Wind;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                day1Button.Content = _weatherData.DailyForecasts[0].Date.Day.ToString();
                day2Button.Content = _weatherData.DailyForecasts[1].Date.Day.ToString();
                day3Button.Content = _weatherData.DailyForecasts[2].Date.Day.ToString();
                day4Button.Content = _weatherData.DailyForecasts[3].Date.Day.ToString();
                day5Button.Content = _weatherData.DailyForecasts[4].Date.Day.ToString();


                minTempLabel.Content = $"{temperature.Minimum.Value}°{temperature.Minimum.Unit}";
                maxTempLabel.Content = $"{temperature.Maximum.Value}°{temperature.Maximum.Unit}";

                windSpeedLabel.Content = $"{wind.Speed.Value} {wind.Speed.Unit}";
                windDirectionLabel.Content = $"{wind.Direction.English}";

                int imageNumber = forecast.Icon;

                weatherIcon.Source = icons[imageNumber - 1];

                precipicationLabel.Content = $"{forecast.PrecipitationProbability} %";
            });

        }

        private void WeatherWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _client.UnSubscribeToWeather();
            _client.OnWeatherRecived -= _client_ServerResponseReceived;
        }

        private void _client_ServerResponseReceived(object? sender, string e)
        {
            Debug.WriteLine("message");
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

        private void Day_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            switch (button)
            {
                case var _ when button == day1Button:
                    forecastNum = 0;
                    break;
                case var _ when button == day2Button:
                    forecastNum = 1;
                    break;
                case var _ when button == day3Button:
                    forecastNum = 2;
                    break;
                case var _ when button == day4Button:
                    forecastNum = 3;
                    break;
                case var _ when button == day5Button:
                    forecastNum = 4;
                    break;
                default:
                    break;
            }
        }

        private void Light_Click(object sender, RoutedEventArgs e)
        {
            isDay = true;
        }

        private void Dark_Click(object sender, RoutedEventArgs e)
        {
            isDay = false;
        }

    }
}
