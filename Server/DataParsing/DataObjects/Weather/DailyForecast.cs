namespace Server.DataParsing.DataObjects.Weather
{
    public class DailyForecast
    {
        public DateTime Date { get; set; }
        public long EpochDate { get; set; }
        public Sun Sun { get; set; }
        public Moon Moon { get; set; }
        public Temperature Temperature { get; set; }
        public RealFeelTemperature RealFeelTemperature { get; set; }
        public double HoursOfSun { get; set; }
        public List<AirAndPollen> AirAndPollen { get; set; }
        public Day Day { get; set; }
        public Night Night { get; set; }
        public List<string> Sources { get; set; }
        public string MobileLink { get; set; }
        public string Link { get; set; }
    }





}
