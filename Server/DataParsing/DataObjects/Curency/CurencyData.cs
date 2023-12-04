using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server.DataParsing.DataObjects.Curency
{
    [JsonArray]
    internal class CurencyData
    {
        
        public CurencyRate[] curencyRates;
    }
}
