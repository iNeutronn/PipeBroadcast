using Server.DataParsing.DataObjects.Shares;
using System;
using System.Collections.Generic;
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
using Server.DataParsing.DataObjects.Shares;
using Newtonsoft.Json;
using System.Diagnostics;
using Server.DataParsing.DataObjects.Curency;
using Server.DataParsing.DataObjects.Weather;
using System.Threading;
using LiveCharts;
using LiveCharts.Wpf;

namespace Client
{
    /// <summary>
    /// Interaction logic for SharesWindow.xaml
    /// </summary>
    public partial class SharesWindow : Window
    {
        private TradingData _tradingData;
        private ClientPipe _client;

        private SeriesCollection SeriesCollection1;
        public Dictionary<DateTime, TradingDataPoint> OLVData => _tradingData.TimeSeries;

        public SharesWindow(ClientPipe client)
        {
            InitializeComponent();
            _client = client;
            _client.OnSharesRecived += _client_ServerResponseReceived;
            Closing += SharesWindow_Closing;
            RewriteInterfacePeriodically();
        }

        private void SharesWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _client.UnSubscribeToShares();
            _client.OnSharesRecived -= _client_ServerResponseReceived;
        }

        private void _client_ServerResponseReceived(object? sender, string e)
        {
            try
            {
                var tradingData = JsonConvert.DeserializeObject<TradingData>(e);

                if (tradingData != null)
                {
                    _tradingData = tradingData;
                }
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"Error deserializing JSON: {ex.Message}");
                // Handle the error as needed
            }
        }



        private void RewriteInterfacePeriodically()
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    RewriteInterface();

                    Thread.Sleep(1000);
                }
            });
            t.Name = "RewriteInterface";
            t.Start();
        }

        private void RewriteInterface()
        {
            if (_tradingData == null) return;

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (SeriesCollection1 == null) SeriesCollection1 = new SeriesCollection();

                SeriesCollection1.Clear();

                var ticks = ConvertDictionaryToLong(OLVData);

                SeriesCollection1.Add(new LineSeries
                {
                    Values = new ChartValues<double>(ticks)
                });

            });

        }

        public static List<double> ConvertDictionaryToLong(Dictionary<DateTime, TradingDataPoint> nullableDictionary)
        {
            List<double> ticksList = new();

            foreach (var kvp in nullableDictionary.Values)
            {
                if (kvp == null)
                {
                    break; // Зупинити обробку, якщо зустріли null
                }

                double ticks = kvp.Open;
                ticksList.Add(ticks);
            }

            return ticksList;
        }

        private void Unsubscribe_Click(object sender, RoutedEventArgs e) => Close();
    }
}
