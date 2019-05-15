using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            WebsocketServer websocketServer = new WebsocketServer();
            websocketServer.Start("http://localhost:2587/test/");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
