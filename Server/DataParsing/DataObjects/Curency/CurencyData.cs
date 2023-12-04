using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.DataParsing.DataObjects.Curency
{
    [JsonArray]
    internal class CurencyData
    {
        
        public CurencyRate[] curencyRates;
    }
}
