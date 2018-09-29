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
        public static Random rand = new Random();
        public static byte[] DataForServer;
        static void Main(string[] args)
        {
            Console.WriteLine("Waiting for connection");
            Thread ListenTherad = new Thread(Listen);
            ListenTherad.Start();
            Thread TalkThread = new Thread(Talk);
            TalkThread.Start();
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
        }
        static void ReceivedAudioHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Klient otrzymał AUDIO od serwera");
            //============//
        }
        static void ReceivedSteeringHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Klient otrzymał STEROWANIE od serwera");
            //============//
        }
        public static void Talk()
        {
            while (true)
            {
                if (Send())
                {
                    count++;
                    //Console.WriteLine($"{count} Packets have been sent");
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.WriteLine($"Error whule sending packet!", ConsoleColor.Red);
                    Thread.Sleep(30);
                }
            }
        }
        public static bool Send()
        {

            int randomInt = rand.Next(0, 3);

            if (randomInt == 0)
            {
                DataForServer = Encoding.ASCII.GetBytes("0123456789012345678901234567890123456789");
            }
            else if (randomInt == 1)
            {
                DataForServer = Encoding.ASCII.GetBytes("01234567890123456789");
            }
            else if (randomInt == 2)
            {
                DataForServer = Encoding.ASCII.GetBytes("0123456789");
            }
            else
            {
                DataForServer = Encoding.ASCII.GetBytes("0");
            }



            try
            {
                client.Send(DataForServer, DataForServer.Length, remoteip);
                return true;
            }
            catch (Exception sysex)
            {

                return false;
            }
        }
    }
}
