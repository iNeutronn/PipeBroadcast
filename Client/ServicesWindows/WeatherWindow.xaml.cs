﻿using Newtonsoft.Json;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for WeatherWindow.xaml
    /// </summary>
    public partial class WeatherWindow : Window
    {
        private WhetherForecast _weatherData;
        private ClientPipe _client;
        private List<BitmapImage> icons = new List<BitmapImage>();
        private int forecastNum = 0;

        public WeatherWindow(ClientPipe client)
        {
            InitializeComponent();
            _client = client;
            _client.OnWeatherRecived += _client_ServerResponseReceived;
            Closing += WeatherWindow_Closing;

            for (int imageNumber = 1; imageNumber <= 44; imageNumber++)
            {
                var str = (imageNumber < 10) ? ("0" + imageNumber.ToString()) : imageNumber.ToString();

                icons.Add(new BitmapImage(new Uri(@$"https://developer.accuweather.com/sites/default/files/{str}-s.png", UriKind.RelativeOrAbsolute)));
            }
            
            RewriteInterfacePeriodically();
        }

        private void RewriteInterfacePeriodically()
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    RewriteInterface();

                    Thread.Sleep(500);
                }
            });
            t.Name = "RewriteInterface";
            t.IsBackground = true;
            t.Start();
        }

        private void RewriteInterface()
        {
            if (_weatherData == null) return;

            var forecast = _weatherData.DailyForecasts[forecastNum].Day;

            var wind = forecast.Wind;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                windSpeedLabel.Content = $"{wind.Speed.Value} {wind.Speed.Unit} {wind.Direction.English}";

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

            forecastNum = int.Parse(((string)button.Content)[3..]) - 1;
        }
    }
}
