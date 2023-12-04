using Newtonsoft.Json;
using Server.DataParsing.DataObjects.Curency;
using Server.DataParsing.DataObjects.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataParsing
{
    internal class CurencyDataParser : CashedDataParser<CurencyData>
    {
        WebClient webClient = new WebClient();
        public CurencyDataParser(TimeSpan timeOut) : base(timeOut)
        {
        }

        protected override CurencyData GetDataFromSource()
        {
            string jsonUrl = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
            string json = webClient.DownloadString(jsonUrl);
            var curencyData = JsonConvert.DeserializeObject<CurencyRate[]>(json)!;
            return new CurencyData { curencyRates = curencyData};
        }
    }
}
