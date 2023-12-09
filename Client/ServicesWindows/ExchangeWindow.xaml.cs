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
using Newtonsoft.Json;
using Server.DataParsing.DataObjects.Curency;

namespace Client
{
    public partial class ExchangeWindow : Window
    {
        private CurencyData _currencyData;
        private ClientPipe _client;

        public ExchangeWindow(ClientPipe client)
        {
            InitializeComponent();
            _client = client;
            _client.OnCurrencyRecived += _client_ServerResponseReceived;
        }

        private void _client_ServerResponseReceived(object? sender, string e)
        {
            try
            {
                var currencyData = JsonConvert.DeserializeObject<CurencyData>(e);

                if (currencyData != null)
                {
                    _currencyData = currencyData;
                }
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"Error deserializing JSON: {ex.Message}");
                // Handle the error as needed
            }
        }

        private void Unsubscribe_Click(object sender, RoutedEventArgs e)
        {
            _client.UnSubscribeToCurrency();
            _client.OnCurrencyRecived -= _client_ServerResponseReceived;
            Close();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}
