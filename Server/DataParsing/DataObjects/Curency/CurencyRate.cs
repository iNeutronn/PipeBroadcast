using Newtonsoft.Json;

namespace Server.DataParsing.DataObjects.Curency
{
    internal class CurencyRate
    {
        [JsonProperty("r030")]
        public int Id { get; set; }
        [JsonProperty("txt")]
        public string Name { get; set; }
        [JsonProperty("rate")]
        public double Rate { get; set; }
        [JsonProperty("cc")]
        public string Code { get; set; }
        [JsonProperty("exchangedate")]
        public DateTime ExchangeDate { get; set; }
    }
}
