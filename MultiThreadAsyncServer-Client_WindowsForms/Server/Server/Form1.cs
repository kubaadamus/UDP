using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Server
{
    public partial class Form1 : Form
    {
        public static IPEndPoint ep = new IPEndPoint(IPAddress.Parse("89.229.95.152"), 16010);
        public static Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        delegate void ShowMessageMethod(byte[] msg);
        public Form1()
        {
            InitializeComponent();
            
            Thread TalkThread = new Thread(Talk);
            TalkThread.Start();
            Thread ListenThread = new Thread(Listen);
            ListenThread.Start();
        }
        public static void Talk()
        {
            while (true)
            {
                string test = DateTime.Now.ToString();
                Send(Encoding.ASCII.GetBytes(test));
                Thread.Sleep(40);
            }
        }
        public void ShowMsg(byte[] msg)
        {
            textBox1.AppendText(Encoding.ASCII.GetString(msg));
        }
        public void Listen()
        {
            while (true)
            {
                if (sock.Available > 0)
                {
                    byte[] ReceivedBytes = new byte[sock.Available];
                    sock.Receive(ReceivedBytes);
                    Console.WriteLine(Encoding.ASCII.GetString(ReceivedBytes));
                    this.Invoke(new ShowMessageMethod(ShowMsg), ReceivedBytes);
                    if (ReceivedBytes.Length > 30)
                    {
                        Thread ReceivedVideo = new Thread(() => ReceivedVideoHandler(ReceivedBytes));
                        ReceivedVideo.Start();
                    }
                    else if (ReceivedBytes.Length == 20)
                    {
                        Thread ReceivedAudio = new Thread(() => ReceivedAudioHandler(ReceivedBytes));
                        ReceivedAudio.Start();
                    }
                    else if (ReceivedBytes.Length == 10)
                    {
                        Thread ReceivedSteering = new Thread(() => ReceivedSteeringHandler(ReceivedBytes));
                        ReceivedSteering.Start();
                    }
                }
            }
        }
        public static bool Send(byte[] DataForClient)
        {
            try
            {
                sock.SendTo(DataForClient, ep);
                return true;
            }
            catch (Exception sysex) { return false; }
        }
        static void ReceivedVideoHandler(byte[] receivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral video od klienta");
            //============//
        }
        static void ReceivedAudioHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral audio od klienta");
            //============//
        }
        static void ReceivedSteeringHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral sterowanie od klienta");
            //============//
        }
    }
}
