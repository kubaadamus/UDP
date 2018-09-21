using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO.Ports;

namespace UDPServer
{
    class Program
    {
        public static IPEndPoint ep = new IPEndPoint(IPAddress.Parse("89.229.95.152"), 16010);
        public static Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public static byte[] ImageArray; // Tablica która zostanie zapełniona danymi z kamerki i wyslana w sieć
            //zmienne portu com//
            public static SerialPort serial1;
            public static string inString = "";
            public static string myString = "";
            //=================//
        public static void Main(string[] args)
        {
            Instantiate();
            Thread.Sleep(1000);
            while (true)
            {
                if(sock!=null)
                {
                    try
                    {
                        byte[] risiw = new byte[20];
                        sock.Receive(risiw);
                        string StringOdKlienta = Encoding.ASCII.GetString(risiw);
                        Console.WriteLine("String od klienta: "+StringOdKlienta + " " + StringOdKlienta.Length);
                        WyslijDoArduino(StringOdKlienta);



                        

                        //REAKCJA PROGRAMU NA STRING OTRZYMANY OD KLIENTA
                        if(StringOdKlienta.Contains("cam0"))
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
                        if (StringOdKlienta.Contains("cam3"))
                        {
                            try
                            {
                                videoSource.Stop();
                                videoSource = new VideoCaptureDevice(videoDevicesList[3].MonikerString);
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

                if (Send(ImageArray))
                {
                    Thread.Sleep(50);
                }
                else
                {
                    Thread.Sleep(50);
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
            ImageArray = ImageToByteArray(bitmap, 20L);   // OBRAZ JEST TU KOMPRESOWANY!
            { }
            //================================================//

            if (Send(ImageArray))
            {
                Console.WriteLine("Wyslano obrazk");
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


                imageIn.Save(ms, imageCodec,parameters);
                return ms.ToArray();

            }
        }
        public static void InicjalizujSerial()
        {

            serial1 = new SerialPort();
            serial1.PortName = "COM4";
            serial1.Parity = Parity.None;
            serial1.BaudRate = 115200;
            serial1.DataBits = 8;
            serial1.StopBits = StopBits.One;
            if (!serial1.IsOpen && serial1 != null)
            {
                serial1.Open();
                serial1.ReadTimeout = 2000;
                serial1.WriteTimeout = 1000;
            }
            serial1.BaseStream.Flush();
            serial1.DiscardInBuffer();
            serial1.DiscardOutBuffer();
        }
        public static void WyslijDoArduino(string inputString)
        {
            serial1.Write(inputString);

        }

        public static void port_OnReceiveDatazz(object sender,
                                          SerialDataReceivedEventArgs e)
        {

            byte[] buf = new byte[serial1.BytesToRead];
            serial1.Read(buf, 0, buf.Length);

            //Odeslij klientowi to co odpowiedzialo Arduino
            Send(buf);

            myString = System.Text.Encoding.ASCII.GetString(buf).Trim();
            Console.WriteLine("Odebrano z arduino: " + myString);



        }
        public static void Instantiate()
        {
            InicjalizujSerial();
            //Przypisanie eventu do serial portu//
            serial1.DataReceived += new SerialDataReceivedEventHandler(port_OnReceiveDatazz);
            //=================================//

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
    }
}
