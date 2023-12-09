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

namespace Client
{
    /// <summary>
    /// Interaction logic for SharesWindow.xaml
    /// </summary>
    public partial class SharesWindow : Window
    {
        private TradingData _tradingData;
        private ClientPipe _client;

        public SharesWindow(ClientPipe client)
        {
            InitializeComponent();
            _client = client;
            _client.OnSharesRecived += _client_ServerResponseReceived; 
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

        private void Unsubscribe_Click(object sender, RoutedEventArgs e)
        {
            _client.UnSubscribeToShares();
            _client.OnSharesRecived -= _client_ServerResponseReceived;
            Close();
        }
    }
}
