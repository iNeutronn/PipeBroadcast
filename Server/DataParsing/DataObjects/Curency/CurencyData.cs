using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Server.DataParsing.DataObjects.Curency
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    [JsonArray]
    internal class CurencyData
    {
        
        public CurencyRate[] curencyRates;
    }
}
