using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
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
using Newtonsoft.Json;
using Server.DataParsing.DataObjects.Curency;

namespace Client
{
    public partial class ExchangeWindow : Window
    {
        public CurrencyManager CurrencyManager { get; set; }
        private ClientPipe _client;

        public ObservableCollection<CurencyRate> CurrencyRates1 { get; set; }

        public ExchangeWindow(ClientPipe client)
        {
            InitializeComponent();
            DataContext = this;
            _client = client;
            _client.OnCurrencyRecived += _client_ServerResponseReceived;
            Closing += ExchangeWindow_Closing;
            CurrencyManager = new CurrencyManager();
            CurrencyRates1 = new ObservableCollection<CurencyRate>();
            CurrencyRates1.Add(new CurencyRate()
            {
                Code = "FD",
                ExchangeDate = DateTime.Now,
                Id = 1,
                Name = string.Empty,
                Rate = 0.0
            });
        }

        private void ExchangeWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _client.UnSubscribeToCurrency();
            _client.OnCurrencyRecived -= _client_ServerResponseReceived;
        }

        private void _client_ServerResponseReceived(object? sender, string e)
        {
            Debug.WriteLine("message");
            try
            {
                var currencyData = JsonConvert.DeserializeObject<CurencyData>(e);

                if (currencyData != null)
                {
                    CurrencyManager.SetNewInfo(currencyData.curencyRates);
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
            Close();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
