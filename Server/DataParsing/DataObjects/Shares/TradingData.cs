using Newtonsoft.Json;

namespace Server.DataParsing.DataObjects.Shares
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    internal class TradingData
    {
        [JsonProperty("Meta Data")]
        public MetaDate MetaDate { get; set; }
        [JsonProperty("Time Series (1min)")]
        public Dictionary<string, TradingDataPoint> TimeSeries { get; set; }

    }
}
