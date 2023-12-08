using Server.DataParsing.DataObjects.Shares;
using System.Net;

namespace Server.DataParsing
{
    internal class SharesDataParser : CashedDataParser<TradingData>, IDisposable
    {
        public static readonly TimeSpan DefoultTimeOut = TimeSpan.FromMinutes(1);
        private static readonly string _apikey = "W5JXT814B7SZ3MGZ";
        private static readonly string _function = "TIME_SERIES_INTRADAY";
        private static readonly string _symbol = "IBM";
        private static readonly string _interval = "1min";
        private WebClient _webClient = new WebClient();

        public SharesDataParser(TimeSpan? timeOut = null) : base(timeOut ?? DefoultTimeOut)
        {
        }

        public void Dispose()
        {
            ((IDisposable)_webClient).Dispose();
        }

        protected override TradingData GetDataFromSource()
        {
            string QUERY_URL = $"https://www.alphavantage.co/query?function={_function}&symbol={_symbol}&interval={_interval}&apikey={_apikey}";
            string json = _webClient.DownloadString(QUERY_URL);
            TradingData data = Newtonsoft.Json.JsonConvert.DeserializeObject<TradingData>(json)!;
            return data;
        }
    }
}
