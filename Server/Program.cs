using Server.DataParsing;

SharesTest();// no needed / can be changed to any other function


void WheatherTest()
{
    WeatherDataParser parser = new WeatherDataParser(TimeSpan.FromMinutes(5));
    var data = parser.GetData();
    Console.WriteLine(data.Headline.Text);

}
void CurencyTest()
{
    CurencyDataParser parser = new CurencyDataParser(TimeSpan.FromMinutes(5));
    var data = parser.GetData();
    Console.WriteLine(data);
}
void SharesTest()
{
    SharesDataParser parser = new SharesDataParser(TimeSpan.FromMinutes(5));
    var data = parser.GetData();
    Console.WriteLine(data);
}