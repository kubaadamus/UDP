using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace UDPServer
{
    class Program
    {
        public static string ip;
        public static int count = 0;
        public static int port = 16010;

        public static Random rand = new Random();
        public static byte[] DataForClient;
        public static IPEndPoint ep = new IPEndPoint(IPAddress.Parse("89.229.95.152"), 16010);
        public static Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        static void Main(string[] args)
        {
            Thread TalkThread = new Thread(Talk);
            TalkThread.Start();
            Thread ListenThread = new Thread(Listen);
            ListenThread.Start();


        }
        public static void Talk()
        {
            while (true)
            {
                if (Send())
                {
                    count++;
                    Console.WriteLine($"{count} Packets have been sent");
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.WriteLine($"Error whule sending packet!", ConsoleColor.Red);
                    Thread.Sleep(30);
                }
            }
        }

        public static void Listen()
        {
            while(true)
            {
                if(sock.Available>0)
                {
                    byte[] ReceivedBytes = new byte[sock.Available];
                    sock.Receive(ReceivedBytes);
                    //Console.WriteLine(Encoding.ASCII.GetString(ReceivedBytes));
                    if (Encoding.ASCII.GetString(ReceivedBytes) == "ClientReceived_VIDEO")
                    {
                        Thread ReceivedVideo = new Thread(() => ReceivedVideoHandler(ReceivedBytes));
                        ReceivedVideo.Start();
                    }
                    else if (Encoding.ASCII.GetString(ReceivedBytes) == "ClientReceived_AUDIO")
                    {
                        Thread ReceivedAudio = new Thread(() => ReceivedAudioHandler(ReceivedBytes));
                        ReceivedAudio.Start();
                    }
                    else if (Encoding.ASCII.GetString(ReceivedBytes) == "ClientReceived_STEERING")
                    {
                        Thread ReceivedSteering = new Thread(() => ReceivedSteeringHandler(ReceivedBytes));
                        ReceivedSteering.Start();
                    }
                }
            }
        }
        public static bool Send()
        {

            int randomInt = rand.Next(0, 3);

            if (randomInt == 0)
            {
                DataForClient = Encoding.ASCII.GetBytes("0123456789012345678901234567890123456789");
            }
            else if (randomInt == 1)
            {
                DataForClient = Encoding.ASCII.GetBytes("01234567890123456789");
            }
            else if (randomInt == 2)
            {
                DataForClient = Encoding.ASCII.GetBytes("0123456789");
            }
            else
            {
                DataForClient = Encoding.ASCII.GetBytes("0");
            }



            try
            {
                sock.SendTo(DataForClient, ep);
                return true;
            }
            catch (Exception sysex)
            {

                return false;
            }
        }
        static void ReceivedVideoHandler(byte[] receivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral video od klienta");
            //============//
            byte[] DataForServer = Encoding.ASCII.GetBytes("ServerReceived_VIDEO_from_client");
        }
        static void ReceivedAudioHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral audio od klienta");
            //============//
            byte[] DataForServer = Encoding.ASCII.GetBytes("ServerReceived_AUDIO_from_client");
        }
        static void ReceivedSteeringHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral sterowanie od klienta");
            //============//
            byte[] DataForServer = Encoding.ASCII.GetBytes("ServerReceived_STEERING_from_client");
        }

    }
}
