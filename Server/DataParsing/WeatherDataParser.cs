using Server.DataParsing.DataObjects.Weather;
using System.Net;
using Newtonsoft.Json;

namespace Server.DataParsing
{
    internal class WeatherDataParser : CashedDataParser<WhetherForecast>, IDisposable
    {
        private string _apiKey = "viSk4R1eBd7nPMPHImCbcL2rTcUCK8FF";
        private string _cityKey = "324561"; //Lviv
        WebClient webClient = new WebClient();

        public WeatherDataParser(TimeSpan timeOut) : base(timeOut)
        {
        }

        public void Dispose()
        {
            webClient.Dispose();
        }

        protected override WhetherForecast GetDataFromSource()
        {
            string jsonUrl = $"http://dataservice.accuweather.com/forecasts/v1/daily/5day/{_cityKey}?apikey={_apiKey}&metric=true";
            string json = webClient.DownloadString(jsonUrl);
            WhetherForecast weatherData = JsonConvert.DeserializeObject<WhetherForecast>(json)!;
            return weatherData;
        }

        
    }
}
