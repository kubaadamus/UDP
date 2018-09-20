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

namespace UDPServer
{
    class Program
    {
        public static string ip;
        public static int count = 0;
        public static int port = 16010;
        public static string StringToSend = "";
        public static byte[] packetdata;
        public static IPEndPoint ep = new IPEndPoint(IPAddress.Parse("89.229.95.152"), 16010);
        public static Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public static int delay = 1000;
        public static byte[] ImageArray;

        //ZMIENNE KAMERKI
        public static FilterInfoCollection videoDevicesList;
        public static IVideoSource videoSource;
        public static void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Bitmap resized = new Bitmap(bitmap, new Size(bitmap.Width / 3, bitmap.Height / 3));
            Image i = (Image)resized;
            ImageArray = ImageToByteArray(i);
            Image imidz = byteArrayToImage(ImageArray);
            Send(ImageArray);
            { }
        }
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            ImageConverter imageConverter = new System.Drawing.ImageConverter();
            Image image = imageConverter.ConvertFrom(byteArrayIn) as Image;

            return image;
        }
        public static void Instantiate()
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
        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {

            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }

        }
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Bmp);
                return ms.ToArray();
            }
        }
        static void Main(string[] args)
        {
            Instantiate();
            Thread.Sleep(5000);
                while (true)
                {
                    if (Send(ImageArray))
                    {
                        count++;
                        //Console.WriteLine($"{count} Packets have been sent, B:" + ImageArray[0] + " G:"+ ImageArray[1]+" R:"+ ImageArray[2]);
                    Thread.Sleep(delay); // 25Hz
                    }
                    else
                    {
                        //Console.WriteLine($"Error whule sending packet!", ConsoleColor.Red);
                    Thread.Sleep(delay); 
                }

                }
            Console.WriteLine("SKONCZYLEM!");
            Console.ReadLine();
            
        }
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
