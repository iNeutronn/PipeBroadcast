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
        WeatherForecast _weatherForecast;
        ClientPipe _client;
        public WeatherWindow(ClientPipe client)
        {
            InitializeComponent();
            _client = client;
            _client.ServerResponseReceived += _client_ServerResponseReceived;
        }

        private void _client_ServerResponseReceived(object? sender, string e)
        {
            Debug.WriteLine("message");
            _weatherForecast = JsonConvert.DeserializeObject<WeatherForecast>(e);
        }
    }
}
