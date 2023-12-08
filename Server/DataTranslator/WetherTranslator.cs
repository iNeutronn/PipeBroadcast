using System;
using System.Text;
using Server.DataParsing;
using Server.DataParsing.DataObjects.Weather;
using Newtonsoft.Json;

namespace Server.DataTranslators
{
    internal class WetherTranslator : DataTranslator<WhetherForecast>
    {
        public WetherTranslator(ICashedDataParser<WhetherForecast> data, TimeSpan interval, Client client) :
            base(data, interval, client){ }

        protected override void Translate(object state)
        {
            lock (_pipeLock)
            {
                
                    if (_client.IsSubscribedToWeather)
                    {
                        _client.SendAnswer(JsonConvert.SerializeObject(_data.GetDataFromSource(),Formatting.Indented));
                    }
                
            }
        }
    }

}

