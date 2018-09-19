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
        
        static void Main(string[] args)
        {

                GetInfo();

                while (true)
                {
                    if (Send())
                    {
                        count++;
                        Console.WriteLine($"{count} Packets have been sent");
                    Thread.Sleep(5);
                    }
                    else
                    {
                        Console.WriteLine($"Error whule sending packet!", ConsoleColor.Red);
                    Thread.Sleep(5);
                }
                    if(count>=10000)
                {
                    break;
                }
                }
            
        }
        public static void GetInfo()
        {
            ip = "89.229.95.152";
            port = 16010;
            Console.WriteLine("IP");
            Console.Write("Port:");

        }
        public static bool Send()
        {
            byte[] packetdata = new byte[10000];
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                sock.SendTo(packetdata, ep);
                return true;
            }
            catch (Exception sysex)
            {

                return false;
            }
        }
    }
}
