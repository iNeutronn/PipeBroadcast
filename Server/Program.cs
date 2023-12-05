using Server.DataParsing;
using Server;
using System.Runtime.ConstrainedExecution;

main();

void main()
{
    ClientManager server = new ClientManager();
    while (true)
    {
        Thread.Sleep(1000000000);
    }
}