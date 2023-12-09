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
            lock (_client)
            {
                if (_client.IsSubscribedToShares)
                {
                    try
                    {
                        TradingData tradingData = _data.GetData();
                        _client.SendAnswer(

                            new TransitionObject()
                            {
                                Data = JsonConvert.SerializeObject(tradingData, Formatting.Indented),
                                Header = "SharesData" 
                            }
                            
                            );
                    }
                    catch(WebException ex)
                    {
                        //_client.SendAnswer("you have problems with Internet.");    //TODO: make a normal error message
                    }
                }
            }
        }
    }
}
