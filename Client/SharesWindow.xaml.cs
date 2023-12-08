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

namespace Client
{
    /// <summary>
    /// Interaction logic for SharesWindow.xaml
    /// </summary>
    public partial class SharesWindow : Window
    {
        TradingData _tradingData;
        ClientPipe _client;

        public SharesWindow(ClientPipe client)
        {
            InitializeComponent();
            _client = client;
            _client.ServerResponseReceived += _client_ServerResponseReceived; 
        }

        private void _client_ServerResponseReceived(object? sender, string e)
        {
            Debug.WriteLine("message");
            _tradingData = JsonConvert.DeserializeObject<TradingData>(e);
        }
    }
}
