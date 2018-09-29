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
using AForge.Video;
using AForge.Video.DirectShow;
using System.IO;
using System.Drawing.Imaging;
using NAudio.Wave;

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
            //StartRecordin();
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

                    if (ReceivedBytes.Length > 30 && ReceivedBytes.Length !=17640)
                    {
                        Thread ReceivedVideo = new Thread(() => ReceivedVideoHandler(ReceivedBytes));
                        ReceivedVideo.Start();
                    }
                    else if (ReceivedBytes.Length == 17640)
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
        //================================================ R E C E I V E D  D A T A =====================================//
        void ReceivedVideoHandler(byte[] receivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral video od klienta");
            this.Invoke(new ShowMessageMethod(ShowVideoMsg), receivedBytes);
            //============//
        }
        void ReceivedAudioHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral audio od klienta");
            this.Invoke(new ShowMessageMethod(ShowAudioMsg), reveivedBytes);
            //============//
        }
        void ReceivedSteeringHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Server odebral sterowanie od klienta");
            this.Invoke(new ShowMessageMethod(ShowSteeringMsg), reveivedBytes);
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
            textBox1.AppendText("Otrzymano AUDIO :D ");
            textBox1.AppendText(Environment.NewLine);
            PlayReceivedAudio(msg);
        }
        public void ShowSteeringMsg(byte[] msg)
        {
            textBox1.AppendText("Otrzymano STEROWANIE :D ");
            textBox1.AppendText(Environment.NewLine);
        }
        //===============================================================================================================//

        //===================================== V I D E O  D E V I C E ==================================================//
        public FilterInfoCollection videoDevicesList;
        public IVideoSource videoSource;
        public byte[] ImageArray; // Tablica która zostanie zapełniona danymi z kamerki i wyslana w sieć
        public long VideoQuality = 60L;
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
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            ImageConverter imageConverter = new System.Drawing.ImageConverter();
            Image image = imageConverter.ConvertFrom(byteArrayIn) as Image;

            return image;
        }
        //===============================================================================================================//

        //=====================================A U D I O  D E V I C E ===================================================//
        public WaveInEvent waveInStream;
        public WaveOut audioout = new WaveOut();
        public WaveFormat wf = new WaveFormat();
        public byte[] AudioArray; //Tablica do której recorder audio będzie wpychał bajty z mikrofonu;
        public bool MonitorAudioInput = false;
        //================================================RECORD==========================================================//
        public void StartRecordin()
        {
            audioout.DesiredLatency = 100;
            waveInStream = new WaveInEvent();
            waveInStream.DeviceNumber = 0;
            waveInStream.WaveFormat = new WaveFormat(44100, 2);
            waveInStream.DataAvailable += new EventHandler<WaveInEventArgs>(waveInStream_DataAvailable);
            waveInStream.StartRecording();
        }
        public void waveInStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            AudioArray = e.Buffer;
            if (MonitorAudioInput)
            {
                Task.Factory.StartNew(() =>
                {
                    

                    //TUTAJ JEST MIEJSCE NA KONWERSJĘ!
                    //===================================
                    Console.WriteLine("Mam mikrofon" + AudioArray.Length);
                    using (WaveOut audioout = new WaveOut())
                    using (MemoryStream ms = new MemoryStream(AudioArray))
                    {
                        ManualResetEvent semaphoreObject = new ManualResetEvent(false);
                        audioout.DesiredLatency = 100;
                        RawSourceWaveStream rsws = new RawSourceWaveStream(ms, wf);
                        IWaveProvider provider = rsws;
                        audioout.Init(provider);
                        EventHandler<NAudio.Wave.StoppedEventArgs> handler = (o, k) =>
                        {
                            semaphoreObject.Set();
                        };
                        audioout.PlaybackStopped += handler;
                        audioout.Play();
                        //while (audioout.PlaybackState != PlaybackState.Stopped) ;
                        semaphoreObject.WaitOne();
                        audioout.PlaybackStopped -= handler;

                    }
                });
            }
            Send(AudioArray);
        }
        //====================================== P L A Y ========================================================//
        public void PlayReceivedAudio(byte[] ReceivedAudioArray)
        {

            Task.Factory.StartNew(() =>
            {
                audioout.Volume = 1.0f;
                using (WaveOut audioout = new WaveOut())
                using (MemoryStream ms = new MemoryStream(ReceivedAudioArray))
                {
                    ManualResetEvent semaphoreObject = new ManualResetEvent(false);
                    audioout.DesiredLatency = 100;
                    RawSourceWaveStream rsws = new RawSourceWaveStream(ms, wf);
                    IWaveProvider provider = rsws;
                    audioout.Init(provider);
                    EventHandler<NAudio.Wave.StoppedEventArgs> handler = (o, k) =>
                    {
                        semaphoreObject.Set();
                    };
                    audioout.PlaybackStopped += handler;
                    audioout.Play();
                    //while (audioout.PlaybackState != PlaybackState.Stopped) ;
                    semaphoreObject.WaitOne();
                    audioout.PlaybackStopped -= handler;
                }
            });
        }
    }
}
