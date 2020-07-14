using System;
using System.Threading;

namespace server1
{
    class Program
    {
        static ServerObject server; // server
        static Thread listenThread; // stream for listetning
        static void Main(string[] args)
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //stream start
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
