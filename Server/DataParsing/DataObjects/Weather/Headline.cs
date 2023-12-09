namespace Server.DataParsing.DataObjects.Weather
{
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    public class Headline
    {
        public DateTime EffectiveDate { get; set; }
        public long EffectiveEpochDate { get; set; }
        public int Severity { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public DateTime EndDate { get; set; }
        public long EndEpochDate { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }

}
