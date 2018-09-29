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
        public UdpClient client = new UdpClient(16010);
        public IPEndPoint remoteip = new IPEndPoint(IPAddress.Any, 16010);
        delegate void ShowMessageMethod(byte[] msg);

        public Form1()
        {
            InitializeComponent();
            Thread ListenThread = new Thread(Listen);
            ListenThread.Start();
        }
        public bool Send(byte[] DataForServer)
        {

            try
            {
                client.Send(DataForServer, DataForServer.Length, remoteip);
                return true;
            }
            catch (Exception sysex) { return false; }
        }
        void Listen()
        {
            while (true)
            {
                byte[] receivedBytes = client.Receive(ref remoteip);
                //Console.WriteLine(Encoding.ASCII.GetString(receivedBytes));
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
        //================================================ R E C E I V E D  D A T A =====================================//
        void ReceivedVideoHandler(byte[] receivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Klient otrzymał VIDEO od serwera");
            this.Invoke(new ShowMessageMethod(ShowVideoMsg), receivedBytes);
            //============//
        }
        void ReceivedAudioHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Klient otrzymał AUDIO od serwera");
            //============//
        }
        void ReceivedSteeringHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Klient otrzymał STEROWANIE od serwera");
            //============//
        }
        public void ShowVideoMsg(byte[] msg)
        {
            textBox1.AppendText("Otrzymano video :D ");
            textBox1.AppendText(Environment.NewLine);
            pictureBox1.Image = byteArrayToImage(msg);
        }
        public void ShowAudioMsg(byte[] msg)
        {
            textBox1.AppendText("Otrzymano video :D ");
            textBox1.AppendText(Environment.NewLine);
        }
        public void ShowSteeringMsg(byte[] msg)
        {
            textBox1.AppendText("Otrzymano video :D ");
            textBox1.AppendText(Environment.NewLine);
        }
        //===============================================================================================================//
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            ImageConverter imageConverter = new System.Drawing.ImageConverter();
            Image image = imageConverter.ConvertFrom(byteArrayIn) as Image;

            return image;
        }
    }
}
