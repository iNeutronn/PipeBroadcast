using System;
using System.Text;
using Server.DataParsing;
using Server.DataParsing.DataObjects.Weather;
using Newtonsoft.Json;

namespace Server.DataTranslators
{
    internal class WetherTranslator : DataTranslator<WhetherForecast>
    {
        public WetherTranslator(CashedDataParser<WhetherForecast> data, TimeSpan interval, List<Client> clients) :
            base(data, interval, clients){ }

        protected override void Translate(object state)
        {
            lock (_pipeLock)
            {
                foreach (var client in _clients)
                {
                    if (client.IsSubscribedToWeather)
                    {
                        client.SendAnswer(JsonConvert.SerializeObject(_data.GetData(),Formatting.Indented));
                    }
                }
            }
        }
    }

}

