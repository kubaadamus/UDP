using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace UDPClient_Simiple
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Waiting for connection");
            UdpClient client = new UdpClient(16010);
            IPEndPoint remoteip = new IPEndPoint(IPAddress.Any, 16010);
            int count = 0;
            while(true)
            {
                byte[] receivedBytes = client.Receive(ref remoteip);

                if (receivedBytes != null)
                {
                    string message = Encoding.ASCII.GetString(receivedBytes);
                    Console.WriteLine(count++ + "received message " );

                }
                else
                {
                    Console.WriteLine("empty message received");
                }
            }


        }
    }
}
