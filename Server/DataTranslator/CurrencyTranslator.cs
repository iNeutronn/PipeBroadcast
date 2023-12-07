using System;
using Newtonsoft.Json;
using Server.DataParsing;
using Server.DataParsing.DataObjects.Shares;
using System.Text;
using Server.DataParsing.DataObjects.Curency;

namespace Server.DataTranslators
{
    internal class CurrencyTranslator : DataTranslator<CurencyData>
    {
        public CurrencyTranslator(CashedDataParser<CurencyData> data, TimeSpan interval, List<Client> clients) :
            base(data, interval, clients)
        { }

        protected override void Translate(object state)
        {
            lock (_pipeLock)
            {
                foreach (var client in _clients)
                {
                    if (client.IsSubscribedToCurrency)
                    {
                        client.SendAnswer(JsonConvert.SerializeObject(_data.GetData(), Formatting.Indented));
                    }
                }
            }
        }
    }
}
