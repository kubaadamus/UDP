using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace UDPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            HandleDataClass hdc = new HandleDataClass();

            Thread serverThread = new Thread(() => server.Listen());

            serverThread.Start();

            Thread dataHandlerThread = new Thread(() => hdc.SubscribeToEvent(server));

            dataHandlerThread.Start();


            while (true)
            {
                Thread.Sleep(100);
            }

        }
    }
}
