using Server.DataParsing;

WheatherTest();// no needed / can be changed to any other function


void WheatherTest()
{
    WeatherDataParser parser = new WeatherDataParser(TimeSpan.FromMinutes(5));
    var data = parser.GetData();
    Console.WriteLine(data.Headline.Text);

}