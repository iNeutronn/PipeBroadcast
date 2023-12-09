using System;
using System.Text;
using Server.DataParsing;
using Server.DataParsing.DataObjects.Weather;
using Newtonsoft.Json;

namespace Server.DataTranslators
{
    internal class WetherTranslator : DataTranslator<WhetherForecast>
    {
        public WetherTranslator(CashedDataParser<WhetherForecast> data, TimeSpan interval, Client client) :
            base(data, interval, client)
        { }

        protected override void Translate(object state)
        {
            lock (_client)
            {

                if (_client.IsSubscribedToWeather)
                {
                    _client.SendAnswer(
               
                             new TransitionObject()
                             {
                                 Data = JsonConvert.SerializeObject(_data.GetData(), Formatting.Indented),
                                 Header = "WeatherData"
                             }
                            
                            );

                }

            }
        }
    }

}

