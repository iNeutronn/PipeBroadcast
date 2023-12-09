using Server.DataParsing.DataObjects.Weather;
using System.Net;
using Newtonsoft.Json;

namespace Server.DataParsing
{
    internal class WeatherDataParser : CashedDataParser<WhetherForecast>, IDisposable
    {
        private string _apiKey = "Dv89Uz6cEitkEt1af3WLY4RzN6pNJJbG"; //"viSk4R1eBd7nPMPHImCbcL2rTcUCK8FF";
        private string _cityKey = "324561"; //Lviv
        WebClient webClient = new WebClient();
        public static readonly TimeSpan DefoultTimeOut = TimeSpan.FromMinutes(30);

        public WeatherDataParser(TimeSpan? timeOut = null) : base(timeOut ?? DefoultTimeOut)
        {

        }

        public void Dispose()
        {
            webClient.Dispose();
        }

        protected override WhetherForecast GetDataFromSource()
        {
            string jsonUrl = $"http://dataservice.accuweather.com/forecasts/v1/daily/5day/{_cityKey}?apikey={_apiKey}&metric=true&details=true";
            string json = webClient.DownloadString(jsonUrl);
            WhetherForecast weatherData = JsonConvert.DeserializeObject<WhetherForecast>(json)!;
            return weatherData;
        }
    }
}
