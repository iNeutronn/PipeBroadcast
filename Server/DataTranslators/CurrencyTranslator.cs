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
        public CurrencyTranslator(CashedDataParser<CurencyData> data, TimeSpan interval, Client client) :
            base(data, interval, client)
        { }

        protected override void Translate(object state)
        {
            lock (_client)
            {

                    if (_client.IsSubscribedToCurrency)
                    {
                    _client.SendAnswer(

                            new TransitionObject()
                            {
                            Data = JsonConvert.SerializeObject(_data.GetData(), Formatting.Indented),
                            Header = "CurrencyData"
                        }
                        
                        );
                    }
               
            }
        }
    }
}
