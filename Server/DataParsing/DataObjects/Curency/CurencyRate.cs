using Newtonsoft.Json;

namespace Server.DataParsing.DataObjects.Curency
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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
