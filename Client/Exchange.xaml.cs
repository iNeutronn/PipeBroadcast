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
    public partial class Exchange : Window
    {
        CurencyData _currencyData;
        ClientPipe _client;

        public Exchange(ClientPipe client)
        {
            InitializeComponent();
            _client = client;
            _client.ServerResponseReceived += _client_ServerResponseReceived;
        }

        private void _client_ServerResponseReceived(object? sender, string e)
        {
            Debug.WriteLine("message");
            _currencyData = JsonConvert.DeserializeObject<CurencyData>(e);
        }
    }

}
