using System;
using Newtonsoft.Json;
using Server.DataParsing;
using Server.DataParsing.DataObjects.Weather;
using System.Text;
using Server.DataParsing.DataObjects.Shares;

namespace Server.DataTranslators
{
    internal class SharesTranslator : DataTranslator<TradingData>
    {
        public SharesTranslator(CashedDataParser<TradingData> data, TimeSpan interval, List<Client> clients) :
            base(data, interval, clients)
        { }

        protected override void Translate(object state)
        {
            lock (_pipeLock)
            {
                foreach (var client in _clients)
                {
                    if (client.IsSubscribedToShares)
                    {
                        client.SendAnswer(JsonConvert.SerializeObject(_data.GetData()));
                    }
                }
            }
        }
    }
}
