using Newtonsoft.Json;
using Server.DataParsing.DataObjects.Curency;
using System.Net;

namespace Server.DataParsing
{
    internal class CurencyDataParser : ICashedDataParser<CurencyData>,IDisposable
    {
        private readonly WebClient webClient = new WebClient();
        public static readonly TimeSpan DefaultTimeOut = TimeSpan.FromMinutes(10);
        private static string NBUAPIUrl = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";

        //public CurencyDataParser(TimeSpan? timeOut = null) : base(timeOut ?? DefaultTimeOut)
        //{
        //}

        public void Dispose()
        {
            ((IDisposable)webClient).Dispose();
        }

        public CurencyData GetDataFromSource()
        {
            
            string json = webClient.DownloadString(NBUAPIUrl);
            var curencyData = JsonConvert.DeserializeObject<CurencyRate[]>(json)!;
            return new CurencyData { curencyRates = curencyData};
        }
    }
}
