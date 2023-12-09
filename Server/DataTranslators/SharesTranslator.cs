using System;
using Newtonsoft.Json;
using Server.DataParsing;
using Server.DataParsing.DataObjects.Weather;
using System.Text;
using Server.DataParsing.DataObjects.Shares;
using System.Net;

namespace Server.DataTranslators
{
    internal class SharesTranslator : DataTranslator<TradingData>
    {
        public SharesTranslator(CashedDataParser<TradingData> data, TimeSpan interval, Client client) :
            base(data, interval, client)
        { }

        protected override void Translate(object state)
        {
            lock (_pipeLock)
            {
                if (_client.IsSubscribedToShares)
                {
                    try
                    {
                        TradingData tradingData = _data.GetData();
                        _client.SendAnswer(JsonConvert.SerializeObject(tradingData, Formatting.Indented));
                    }
                    catch(WebException ex)
                    {
                        _client.SendAnswer("you have problems with Internet.");    
                    }
                }
            }
        }
    }
}
