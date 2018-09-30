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
using System.IO.Ports;
using System.Management;
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
            StartRecordin();
            ConnectArduino();
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

                    if (ReceivedBytes.Length > 1000 && ReceivedBytes.Length !=17640)
                    {
                        Thread ReceivedVideo = new Thread(() => ReceivedVideoHandler(ReceivedBytes));
                        ReceivedVideo.Start();
                    }
                    else if (ReceivedBytes.Length == 17640)
                    {
                        Thread ReceivedAudio = new Thread(() => ReceivedAudioHandler(ReceivedBytes));
                        ReceivedAudio.Start();
                    }
                    else if (Encoding.ASCII.GetString(ReceivedBytes).StartsWith("ARD"))
                    {
                        Thread ReceivedSteering = new Thread(() => ReceivedSteeringHandler(ReceivedBytes));
                        ReceivedSteering.Start();
                    }
                }
            }
        }
        //==================================== R E C E I V E D  D A T A =====================================//
        void ReceivedVideoHandler(byte[] receivedBytes)
        {
            //HDNADLE DATA//
            //Console.WriteLine("Server odebral video od klienta");
            this.Invoke(new ShowMessageMethod(ShowVideoMsg), receivedBytes);
            //============//
        }
        void ReceivedAudioHandler(byte[] receivedBytes)
        {
            //HDNADLE DATA//
            //Console.WriteLine("Server odebral audio od klienta");
            this.Invoke(new ShowMessageMethod(ShowAudioMsg), receivedBytes);
            //============//
        }
        void ReceivedSteeringHandler(byte[] receivedBytes)
        {
            //HDNADLE DATA//
            Console.WriteLine("Komenda od klienta: " + Encoding.ASCII.GetString(receivedBytes));
            this.Invoke(new ShowMessageMethod(ShowSteeringMsg), receivedBytes);
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
            if(IsArduinoConnected)
            {
                WyslijDoArduino(Encoding.ASCII.GetString(msg).Replace("ARD", ""));
            }
            textBox1.AppendText(Encoding.ASCII.GetString(msg) + Environment.NewLine);
        }
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
            catch (Exception) { }
            return null;
        }
        //=====================================A U D I O  D E V I C E ===================================================//
        public WaveInEvent waveInStream;
        public WaveOut audioout = new WaveOut();
        public WaveFormat wf = new WaveFormat();
        public byte[] AudioArray; //Tablica do której recorder audio będzie wpychał bajty z mikrofonu;
        public bool MonitorAudioInput = false;
        //======================================== R E C O R D ==========================================================//
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
                        //while (audioout.PlaybackState != PlaybackState.Stopped) ;
                        semaphoreObject.WaitOne();
                        audioout.PlaybackStopped -= handler;

                    }
                });
            }
            Send(AudioArray);
        }
        //========================================== P L A Y ========================================================//
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
                    semaphoreObject.WaitOne();
                    audioout.PlaybackStopped -= handler;
                }
            });
        }
        //======================================== A R D U I N O ====================================================//
        public string StringOdKlienta = "";
        public string ArduinoPort = "COM3";
        public SerialPort serial1;
        public string inString = "";
        public string myString = "";
        public bool IsArduinoConnected = false;
        public string ConnectArduino()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);
            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();
                    Console.WriteLine("Wykryto: " + desc + " / " + deviceId);
                    if (desc.Contains("Arduino"))
                    {
                        Console.WriteLine(desc);
                        Console.WriteLine(deviceId);
                        ArduinoPort = deviceId;
                        Console.WriteLine("ustawiam port arduino na: " + ArduinoPort);
                        try
                        {
                            InicjalizujSerial();
                            //Przypisanie eventu do serial portu//
                            serial1.DataReceived += new SerialDataReceivedEventHandler(port_OnReceiveDatazz);
                            //=================================//
                            IsArduinoConnected = true;
                        }
                        catch (ManagementException e){/* Do Nothing */}
                        return deviceId;
                    }
                }
            }
            catch (Exception) {
                Console.WriteLine("NIE MA ARDUINO :C ");
                IsArduinoConnected = false;
            }
            Console.WriteLine("NIE MA ARDUINO :C ");
            IsArduinoConnected = false;
            return null;
        }
        public void InicjalizujSerial()
        {

            serial1 = new SerialPort();
            serial1.PortName = ArduinoPort;
            serial1.Parity = Parity.None;
            serial1.BaudRate = 115200;
            serial1.DataBits = 8;
            serial1.StopBits = StopBits.One;
            if (!serial1.IsOpen && serial1 != null)
            {
                serial1.Open();
                serial1.ReadTimeout = 30;
                serial1.WriteTimeout = 30;
            }
            serial1.BaseStream.Flush();
            serial1.DiscardInBuffer();
            serial1.DiscardOutBuffer();
        }
        public void WyslijDoArduino(string inputString)
        {
                serial1.Write(inputString);
        }
        public void port_OnReceiveDatazz(object sender, SerialDataReceivedEventArgs e)
        {

            byte[] buf = new byte[serial1.BytesToRead];
            serial1.Read(buf, 0, buf.Length);
            //Odeslij klientowi to co odpowiedzialo Arduino
            Send(buf);
            myString = System.Text.Encoding.ASCII.GetString(buf).Trim();
            Console.WriteLine("Odebrano z arduino: " + myString);
        }
    }
}
