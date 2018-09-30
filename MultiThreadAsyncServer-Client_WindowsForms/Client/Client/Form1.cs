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
using AForge.Video;
using AForge.Video.DirectShow;
using System.IO;
using System.Drawing.Imaging;
using NAudio.Wave;
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
            Instantiate_Camera();
            StartRecordin();
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
                    if (receivedBytes.Length > 1000 && receivedBytes.Length!=17640)
                    {
                        Thread ReceivedVideo = new Thread(() => ReceivedVideoHandler(receivedBytes));
                        ReceivedVideo.Start();
                    }
                    else if (receivedBytes.Length == 17640)
                    {
                        Thread ReceivedAudio = new Thread(() => ReceivedAudioHandler(receivedBytes));
                        ReceivedAudio.Start();
                    }
                    else if (receivedBytes.Length < 100)
                    {
                        Thread ReceivedSteering = new Thread(() => ReceivedSteeringHandler(receivedBytes));
                        ReceivedSteering.Start();
                    }
                }
        }
        //============================= R E C E I V E D  D A T A =====================================//
        void ReceivedVideoHandler(byte[] receivedBytes)
        {
            //HDNADLE DATA//
            //Console.WriteLine("Klient otrzymał VIDEO od serwera");
            try
            {
                this.Invoke(new ShowMessageMethod(ShowVideoMsg), receivedBytes);
            }
            catch (Exception){}
            //============//
        }
        void ReceivedAudioHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            //Console.WriteLine("Klient otrzymał AUDIO od serwera");
            try
            {
                this.Invoke(new ShowMessageMethod(ShowAudioMsg), reveivedBytes);
            }
            catch (Exception){}
            //============//
        }
        void ReceivedSteeringHandler(byte[] reveivedBytes)
        {
            //HDNADLE DATA//
            //Console.WriteLine("Klient otrzymał STEROWANIE od serwera");
            try
            {
                this.Invoke(new ShowMessageMethod(ShowSteeringMsg), reveivedBytes);
            }
            catch (Exception){}
            //============//
        }
        public void ShowVideoMsg(byte[] msg)
        {
            pictureBox1.Image = byteArrayToImage(msg);
        }
        public void ShowAudioMsg(byte[] msg)
        {
            PlayReceivedAudio(msg);
        }
        public void ShowSteeringMsg(byte[] msg)
        {
            textBox1.AppendText("ARD:"+Encoding.ASCII.GetString(msg));
        }
        //=============================== V I D E O  D E V I C E ==================================================//
        public FilterInfoCollection videoDevicesList;
        public IVideoSource videoSource;
        public byte[] ImageArray; // Tablica która zostanie zapełniona danymi z kamerki i wyslana w sieć
        public long VideoQuality = 5L;
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
            videoSource = new VideoCaptureDevice(videoDevicesList[1].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
            Console.WriteLine("kamerka wybrana: " + videoDevicesList[1].Name);
        }
        public void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            //=========================KONWERTER==============//
            ImageArray = ImageToByteArray(bitmap, VideoQuality);   // OBRAZ JEST TU KOMPRESOWANY!
            //================================================//
            if (Send(ImageArray))
            {
                //Console.WriteLine("Wyslano obrazk:" + ImageArray.Length + "bytes");
            }
            else
            {
                //Console.WriteLine("Cos nie tak z obrazkiem!");
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
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                ImageConverter imageConverter = new System.Drawing.ImageConverter();
                Image image = imageConverter.ConvertFrom(byteArrayIn) as Image;
                return image;
            }
            catch (Exception){}
            return null;
        }
        //===============================A U D I O  D E V I C E===========================================//
        public WaveInEvent waveInStream;
        public WaveOut audioout = new WaveOut();
        public WaveFormat wf = new WaveFormat();
        public byte[] AudioArray; //Tablica do której recorder audio będzie wpychał bajty z mikrofonu;
        public bool MonitorAudioInput = false;
        //===================================== P L A Y ========================================================//
        public void PlayReceivedAudio(byte[] ReceivedAudioArray)
        {            Task.Factory.StartNew(() =>
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
        //==================================== R E C O R D ==========================================================//
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
                    //Console.WriteLine("Mam mikrofon" + AudioArray.Length);
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
                        semaphoreObject.WaitOne();
                        audioout.PlaybackStopped -= handler;
                    }
                });
            }
            Send(AudioArray);
        }
        //=================================== K E Y P R E S S ======================================================//
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        bool W = true; bool S = true; bool A = true; bool D = true;
        bool _But8 = true; bool _But5 = true; bool _But4 = true; bool _But6 = true;
        protected override bool ProcessKeyPreview(ref Message m)
        {
            //========================================= RUCH PLATFORMY =======================================//
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.W && W)
                {
                    W = false; WBut.Enabled = false;
                    byte[] data = Encoding.ASCII.GetBytes("ARD1");
                    client.Send(data, data.Length, remoteip);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.W)
                {
                    W = true; WBut.Enabled = true;
                    byte[] data = Encoding.ASCII.GetBytes("ARD5");
                    client.Send(data, data.Length, remoteip);
                }
            } // W
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.S && S)
                {
                    S = false; SBut.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD2"); client.Send(data, data.Length, remoteip);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.S)
                {
                    S = true; SBut.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD6"); client.Send(data, data.Length, remoteip);
                }
            } // S
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.A && A)
                {
                    A = false; ABut.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD3"); client.Send(data, data.Length, remoteip);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.A)
                {
                    A = true; ABut.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD7"); client.Send(data, data.Length, remoteip);
                }
            } // A
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.D && D)
                {
                    D = false; DBut.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD4"); client.Send(data, data.Length, remoteip);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.D)
                {
                    D = true; DBut.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD8"); client.Send(data, data.Length, remoteip);
                }
            } // D
              //========================================= RUCH GŁOWY =======================================//
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.NumPad8 && _But8)
                {
                    _But8 = false; But8.Enabled = false;
                    byte[] data = Encoding.ASCII.GetBytes("ARD9");
                    client.Send(data, data.Length, remoteip);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.NumPad8)
                {
                    _But8 = true; But8.Enabled = true;
                    byte[] data = Encoding.ASCII.GetBytes("ARD13");
                    client.Send(data, data.Length, remoteip);
                }
            } // But8
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.NumPad5 && _But5)
                {
                    _But5 = false; But5.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD10"); client.Send(data, data.Length, remoteip);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.NumPad5)
                {
                    _But5 = true; But5.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD14"); client.Send(data, data.Length, remoteip);
                }
            } // But5
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.NumPad4 && _But4)
                {
                    _But4 = false; But4.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD11"); client.Send(data, data.Length, remoteip);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.NumPad4)
                {
                    _But4 = true; But4.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD15"); client.Send(data, data.Length, remoteip);
                }
            } // But4
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.NumPad6 && _But6)
                {
                    _But6 = false; But6.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD12"); client.Send(data, data.Length, remoteip);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.NumPad6)
                {
                    _But6 = true; But6.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD16"); client.Send(data, data.Length, remoteip);
                }
            } // But6
            return base.ProcessKeyPreview(ref m);
        }
    }
}
