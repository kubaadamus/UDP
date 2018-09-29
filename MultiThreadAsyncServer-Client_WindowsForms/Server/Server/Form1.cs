﻿using System;
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
using AForge.Video;
using AForge.Video.DirectShow;
using System.IO;
using System.Drawing.Imaging;

namespace Server
{
    public partial class Form1 : Form
    {
        public IPEndPoint ep = new IPEndPoint(IPAddress.Parse("89.229.95.152"), 16010);
        public Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        delegate void ShowMessageMethod(byte[] msg);

        public Form1()
        {
            InitializeComponent();
            Thread ListenThread = new Thread(Listen);
            ListenThread.Start();
            Instantiate_Camera();
        }
        public bool Send(byte[] DataForClient)
        {
            try
            {
                sock.SendTo(DataForClient, ep);
                return true;
            }
            catch (Exception sysex) { return false; }
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
        public void ShowMsg(byte[] msg)
        {
            textBox1.AppendText(Encoding.ASCII.GetString(msg));
            textBox1.AppendText(Environment.NewLine);
        }
        //================================================ R E C E I V E D  D A T A =====================================//
        void ReceivedVideoHandler(byte[] receivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral video od klienta");
            //============//
        }
        void ReceivedAudioHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral audio od klienta");
            //============//
        }
        void ReceivedSteeringHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral sterowanie od klienta");
            //============//
        }
        public void ShowVideoMsg(byte[] msg)
        {
            textBox1.AppendText("Otrzymano video :D ");
            textBox1.AppendText(Environment.NewLine);
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
        //===================================== V I D E O  D E V I C E ==================================================//
        public FilterInfoCollection videoDevicesList;
        public IVideoSource videoSource;
        public byte[] ImageArray; // Tablica która zostanie zapełniona danymi z kamerki i wyslana w sieć
        public long VideoQuality = 10L;
        public void Instantiate_Camera()
        {
            videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo videoDevice in videoDevicesList)
            {
                Console.WriteLine(videoDevice.Name);
            }
            if (videoDevicesList.Count > 0)
            {
                Console.WriteLine("mamy kamerek: " + videoDevicesList.Count);
            }
            else
            {
                Console.WriteLine("Nie mamy kamerek :/");
            }
            videoSource = new VideoCaptureDevice(videoDevicesList[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
            Console.WriteLine("kamerka wybrana: " + videoDevicesList[0].Name);
        }
        public void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            //=========================KONWERTER==============//
            ImageArray = ImageToByteArray(bitmap, VideoQuality);   // OBRAZ JEST TU KOMPRESOWANY!
            //================================================//
            if (Send(ImageArray))
            {
                Console.WriteLine("Wyslano obrazk:" + ImageArray.Length + "bytes");
            }
            else
            {
                Console.WriteLine("Cos nie tak z obrazkiem!");
            }
        }
        public byte[] ImageToByteArray(System.Drawing.Bitmap imageIn, long quality)
        {
            using (var ms = new MemoryStream())
            {
                EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                ImageCodecInfo imageCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameters parameters = new EncoderParameters(1);
                parameters.Param[0] = qualityParam;
                imageIn.Save(ms, imageCodec, parameters);
                return ms.ToArray();
            }
        }
        //===============================================================================================================//
    }
}