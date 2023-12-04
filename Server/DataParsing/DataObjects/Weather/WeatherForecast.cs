namespace Server.DataParsing.DataObjects.Weather
{
    public class WhetherForecast
    {
        public Headline Headline { get; set; }
        public List<DailyForecast> DailyForecasts { get; set; }
    }
}
