using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{

    public partial class Form1 : Form
    {
        public static UdpClient client = new UdpClient(16010);
        public static IPEndPoint remoteip = new IPEndPoint(IPAddress.Any, 16010);
        delegate void ShowMessageMethod(byte[] msg);
        public Form1()
        {
            InitializeComponent();
            Thread TalkThread = new Thread(Talk);
            TalkThread.Start();
            Thread ListenThread = new Thread(Listen);
            ListenThread.Start();
        }
        public void ShowMsg(byte[] msg)
        {
            textBox1.AppendText(Encoding.ASCII.GetString(msg));
        }
        void Listen()
        {
            while (true)
            {
                byte[] receivedBytes = client.Receive(ref remoteip);
                Console.WriteLine(Encoding.ASCII.GetString(receivedBytes));
                this.Invoke(new ShowMessageMethod(ShowMsg), receivedBytes);
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
                string test = DateTime.Now.ToString();
                Send(Encoding.ASCII.GetBytes(test));
                Thread.Sleep(40);
            }
        }
        public static bool Send(byte[] DataForServer)
        {

            try
            {
                client.Send(DataForServer, DataForServer.Length, remoteip);
                return true;
            }
            catch (Exception sysex) { return false; }
        }
    }
}
