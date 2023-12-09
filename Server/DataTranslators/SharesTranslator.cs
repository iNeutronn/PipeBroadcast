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

                    TransitionObject transitionObject;
                    try
                    {
                        transitionObject = new TransitionObject()
                        {
                            Data = JsonConvert.SerializeObject(_data.GetData(), Formatting.Indented),
                            Header = "CurrencyData"
                        };

                    }
                    catch (WebException ex)
                    {
                        transitionObject = new TransitionObject()
                        {
                            Data = ex.Message,
                            Header = "Exception"
                        };
                    }

                    _client.SendAnswer(transitionObject);
                }
            }
        }
    }
}
