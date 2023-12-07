using Newtonsoft.Json;

namespace Server.DataParsing.DataObjects.Shares
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public class MetaDate
    {
        [JsonProperty("1. Information")]
        public string Information { get; set; }
        [JsonProperty("2. Symbol")]
        public string Symbol { get; set; }
        [JsonProperty("3. Last Refreshed")]
        public DateTime LastRefreshed { get; set; }
        [JsonProperty("4. Output Size")]
        public string OutputSize { get; set; }
        [JsonProperty("5. Time Zone")]
        public string TimeZone { get; set; }


    }
}