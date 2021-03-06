﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Management;
using NAudio.Wave;
using AForge.Video;
using AForge.Video.DirectShow;

namespace UDPServer
{
    class Program
    {
        public static WaveInEvent waveInStream;
        public static WaveOut audioout = new WaveOut();
        public static WaveFormat wf = new WaveFormat();
        public static int port_Video = 16012;
        public static IPEndPoint ep;
        public static Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public static byte[] ImageArray; // Tablica która zostanie zapełniona danymi z kamerki i wyslana w sieć
        public static byte[] AudioArray; //Tablica do której recorder audio będzie wpychał bajty z mikrofonu;
        public static long VideoQuality = 50L;
        public static string StringOdKlienta = "";
        public static string ArduinoPort = "COM3";
        public static bool MonitorAudioInput = false;
        //zmienne portu com//
        public static SerialPort serial1;
        public static string inString = "";
        public static string myString = "";
        //=================//
        public static void StartRecordin()
        {
            audioout.DesiredLatency = 100;
            waveInStream = new WaveInEvent();
            waveInStream.DeviceNumber = 0;
            
            waveInStream.WaveFormat = new WaveFormat(44100, 2);
            waveInStream.DataAvailable += new EventHandler<WaveInEventArgs>(waveInStream_DataAvailable);
            waveInStream.StartRecording();

        }
        public static void waveInStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (!MonitorAudioInput)
            {
                audioout.Volume = 0.0f;
            }
            else
            {
                audioout.Volume = 1.0f;
            }
                Task.Factory.StartNew(() =>
                {
                    AudioArray = e.Buffer;

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
                        Send(AudioArray);
                    }
                });

            
        }
        public static void Main(string[] args)
        {
            Console.WriteLine("Wpisz port");
            port_Video = Int16.Parse(Console.ReadLine());
            ep = new IPEndPoint(IPAddress.Parse("89.229.95.152"), port_Video);
            Console.WriteLine("Podaj hasło");
            try
            {
                StartRecordin();
            }
            catch (Exception)
            {
            }

            AutodetectArduinoPort();
            try
            {
                InicjalizujSerial();
                //Przypisanie eventu do serial portu//
                serial1.DataReceived += new SerialDataReceivedEventHandler(port_OnReceiveDatazz);
                //=================================//
            }
            catch (Exception) { Console.WriteLine("NIE MA ARDUINO :C "); }
            try
            {
                Instantiate_Camera();
            }
            catch (Exception)
            { Console.WriteLine("Nie wyłapałem żadnej kamerki ;_;"); }
            

            Thread.Sleep(2000);
            while (true)
            {
                if (sock != null)
                {
                    try
                    {
                        byte[] risiw = new byte[10];
                        sock.Receive(risiw);
                        StringOdKlienta = Encoding.ASCII.GetString(risiw);
                        Console.WriteLine("String od klienta: " + StringOdKlienta + " " + StringOdKlienta.Length);


                        //================================================== SERWER PRZEKAZUJE DANE OD KLIENTA DO ARDUINO ===================================//
                        if (StringOdKlienta.Contains("ARD"))
                        {
                            WyslijDoArduino(StringOdKlienta.Replace("ARD", ""));
                        }
                        //===================================================================================================================================//

                        if (StringOdKlienta.ToLower().Contains('q'))
                        {
                            Console.WriteLine("PRZYCINAM!");
                            string przyciete = (StringOdKlienta.Substring(1, 2));
                            Console.WriteLine(przyciete);
                            int x = Int32.Parse(przyciete);
                            Console.WriteLine(x);
                            VideoQuality = x;
                        }



                        //REAKCJA PROGRAMU NA STRING OTRZYMANY OD KLIENTA
                        if (StringOdKlienta.Contains("cam0"))
                        {
                            try
                            {
                                videoSource.Stop();
                                videoSource = new VideoCaptureDevice(videoDevicesList[0].MonikerString);
                                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                                videoSource.Start();
                                Console.WriteLine("kamerka wybrana: " + videoDevicesList[0].Name);
                            }
                            catch (Exception)
                            {

                            }

                        }
                        if (StringOdKlienta.Contains("cam1"))
                        {
                            try
                            {
                                videoSource.Stop();
                                videoSource = new VideoCaptureDevice(videoDevicesList[1].MonikerString);
                                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                                videoSource.Start();
                                Console.WriteLine("kamerka wybrana: " + videoDevicesList[1].Name);
                            }
                            catch (Exception)
                            {
                            }

                        }
                        if (StringOdKlienta.Contains("cam2"))
                        {
                            try
                            {
                                videoSource.Stop();
                                videoSource = new VideoCaptureDevice(videoDevicesList[2].MonikerString);
                                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                                videoSource.Start();
                                Console.WriteLine("kamerka wybrana: " + videoDevicesList[2].Name);
                            }
                            catch (Exception)
                            {
                            }

                        }

                    }
                    catch (Exception)
                    {
                    }
                }

            }
        }
        //ZMIENNE KAMERKI
        public static FilterInfoCollection videoDevicesList;
        public static IVideoSource videoSource;
        public static void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Bitmap resized = new Bitmap(bitmap, new Size(bitmap.Width / 3, bitmap.Height / 3));
            //=========================KONWERTER==============//
            ImageArray = ImageToByteArray(bitmap, VideoQuality);   // OBRAZ JEST TU KOMPRESOWANY!
            { }
            //================================================//

            if (Send(ImageArray))
            {
                Console.WriteLine("Wyslano obrazk:" + ImageArray.Length + "bytes");
            }
            else
            {
                Console.WriteLine("Cos nie tak z obrazkiem!");
            }

            { }
        }
        //public static Image GetCompressedBitmap(Bitmap bmp, long quality)
        //{
        //    using (var mss = new MemoryStream())
        //    {
        //       EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
        //        ImageCodecInfo imageCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
        //        EncoderParameters parameters = new EncoderParameters(1);
        //        parameters.Param[0] = qualityParam;
        //        bmp.Save(mss, imageCodec, parameters);
        //        return Image.FromStream(mss);
        //    }
        //}
        public static byte[] ImageToByteArray(System.Drawing.Bitmap imageIn, long quality)
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
        public static void InicjalizujSerial()
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
        public static void WyslijDoArduino(string inputString)
        {
            serial1.Write(inputString);

        }
        public static void port_OnReceiveDatazz(object sender,SerialDataReceivedEventArgs e)
        {

            byte[] buf = new byte[serial1.BytesToRead];
            serial1.Read(buf, 0, buf.Length);

            //Odeslij klientowi to co odpowiedzialo Arduino
            Send(buf);

            myString = System.Text.Encoding.ASCII.GetString(buf).Trim();
            Console.WriteLine("Odebrano z arduino: " + myString);


        }
        public static void Instantiate_Camera()
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
        //public static byte[] BitmapToByteArray(Bitmap bitmap)
        //{
        //    BitmapData bmpdata = null;
        //    try
        //    {
        //        bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
        //        int numbytes = bmpdata.Stride * bitmap.Height;
        //        byte[] bytedata = new byte[numbytes];
        //        IntPtr ptr = bmpdata.Scan0;
        //
        //       Marshal.Copy(ptr, bytedata, 0, numbytes);
        //
        //        return bytedata;
        //    }
        //    finally
        //    {
        //        if (bmpdata != null)
        //            bitmap.UnlockBits(bmpdata);
        //    }
        //}
        //public static Image byteArrayToImage(byte[] byteArrayIn)
        //{
        //    ImageConverter imageConverter = new System.Drawing.ImageConverter();
        //    Image image = imageConverter.ConvertFrom(byteArrayIn) as Image;
        //
        //    return image;
        //}
        public static bool Send(byte[] arrayToSend)
        {
            try
            {
                sock.SendTo(arrayToSend, ep);
                return true;
            }
            catch (Exception sysex)
            {

                return false;
            }
        }
        //========================= ZDOBĄDŹ LISTĘ URZĄDZEŃ USB ========================//
        public static string AutodetectArduinoPort()
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
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                /* Do Nothing */
            }

            return null;
        }
        //EENT HANDLER DLA POŁĄCZENIA
    }
}
