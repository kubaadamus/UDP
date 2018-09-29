using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace UDPClient_Simiple
{
    class Program
    {
        public static UdpClient client = new UdpClient(16010);
        public static IPEndPoint remoteip = new IPEndPoint(IPAddress.Any, 16010);
        public static int count = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("Waiting for connection");
            Thread ListenTherad = new Thread(Listen);
            ListenTherad.Start();
        }

        static void Listen()
        {
            while (true)
            {
                byte[] receivedBytes = client.Receive(ref remoteip);

                if (receivedBytes.Length > 30)
                {
                    Thread ReceivedVideo = new Thread(() => ReceivedVideoHandler(receivedBytes));
                    ReceivedVideo.Start();
                }
                else if (receivedBytes.Length == 20)
                {
                    Thread ReceivedAudio = new Thread(() => ReceivedAudioHandler(receivedBytes));
                    ReceivedAudio.Start();
                }
                else if (receivedBytes.Length == 10)
                {
                    Thread ReceivedSteering = new Thread(() => ReceivedSteeringHandler(receivedBytes));
                    ReceivedSteering.Start();
                }


            }
        }

        static void ReceivedVideoHandler(byte[] receivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Klient otrzymał VIDEO od serwera");
            //============//
            byte[] DataForServer = Encoding.ASCII.GetBytes("ClientReceived_VIDEO");
            client.Send(DataForServer, DataForServer.Length, remoteip);
        }
        static void ReceivedAudioHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Klient otrzymał AUDIO od serwera");
            //============//
            byte[] DataForServer = Encoding.ASCII.GetBytes("ClientReceived_AUDIO");
            client.Send(DataForServer, DataForServer.Length, remoteip);
        }
        static void ReceivedSteeringHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Klient otrzymał STEROWANIE od serwera");
            //============//
            byte[] DataForServer = Encoding.ASCII.GetBytes("ClientReceived_STEERING");
            client.Send(DataForServer, DataForServer.Length, remoteip);
        }
    }
}
