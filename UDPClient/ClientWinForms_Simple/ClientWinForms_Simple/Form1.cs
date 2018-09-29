using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;
using NAudio.Wave;

namespace ClientWinForms_Simple
{
    public partial class Form1 : Form
    {
        public static int port_Video = 16010;
        public static int port_Audio = 16012;

        delegate void ShowMessageMethod(byte[] msg);
        UdpClient client = null;
        IPEndPoint endpoint = null;
        Thread listenThread = null;
        private bool isServerStarted = false;
        bool W = true;bool S = true;bool A = true;bool D = true;
        bool _But8 = true; bool _But5 = true; bool _But4 = true; bool _But6 = true;
        public Form1()
        {




            InitializeComponent();
            if (isServerStarted){Stop();}else{Start();}
        }
        private void serverMsgBox_Load(object sender, EventArgs e)
        {
        }
        private void btStart_Click(object sed, EventArgs e)
        {

        }
        private void Start()
        {
            //Create the server.
            IPEndPoint serverEnd = new IPEndPoint(IPAddress.Any, port_Video);
            client = new UdpClient(serverEnd);
            //ShowMsg("Waiting for a client...");
            //Create the client end.
            endpoint = new IPEndPoint(IPAddress.Any, port_Video);

            //Start listening.
            Thread listenThread = new Thread(new ThreadStart(Listening));
            listenThread.Start();
            //Change state to indicate the server starts.
            isServerStarted = true;
        }
        private void Stop()
        {
                //Stop listening.
                listenThread.Join();
                //ShowMsg("Server stops.");
                client.Close();
                //Changet state to indicate the server stops.
                isServerStarted = false;
        }
        private void Listening()
        {
            byte[] data;
            //Listening loop.
            while (true)
            {
                //receieve a message form a client.
                data = client.Receive(ref endpoint);
                //Show the message.
                this.Invoke(new ShowMessageMethod(ShowMsg),data);
                //Send a response message.
                //data = Encoding.ASCII.GetBytes("Dostalem!");
                //client.Send(data, data.Length, endpoint);
                //Sleep for UI to work.;
            }
        }
        private void ShowMsg(byte[] msg)
        {
            if(msg.Length<100)
            {

                if(Encoding.ASCII.GetString(msg)!="0")
                {
                    DebugConsole.AppendText(DateTime.Now + Encoding.ASCII.GetString(msg));
                }
 
            }
            else if(msg.Length==17640)
            {
                waveInStream_DataAvailable(msg);
            }
            else if(msg.Length==3)
            {

            }
            else
            {
                pictureBox1.Image = byteArrayToImage(msg);
            }

        }
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            ImageConverter imageConverter = new System.Drawing.ImageConverter();
            Image image = imageConverter.ConvertFrom(byteArrayIn) as Image;

            return image;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes("cam0");
            client.Send(data, data.Length, endpoint);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes("cam1");
            client.Send(data, data.Length, endpoint);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes("cam2");
            client.Send(data, data.Length, endpoint);
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            DebugConsole.AppendText(hScrollBar1.Value.ToString());
            DebugConsole.AppendText(Environment.NewLine);
            byte[] data = Encoding.ASCII.GetBytes("Q"+hScrollBar1.Value.ToString());
            client.Send(data, data.Length, endpoint);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static WaveInEvent waveInStream;
        public static WaveOut audioout = new WaveOut();
        public static WaveFormat wf = new WaveFormat();
        //AUDIO


        public static void waveInStream_DataAvailable(byte[] ReceivedAudioArray)
        {
            Task.Factory.StartNew(() =>
            {
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

        private void PasswordButton_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes("abcde");
            client.Send(data, data.Length, endpoint);
        }

        private void ArduinoTestButton_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes("ARD2");
            client.Send(data, data.Length, endpoint);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("klawisz");
        }


        //PRZETWARZANIE KLAWISZY

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;

        protected override bool ProcessKeyPreview(ref Message m)
        {

            //========================================= RUCH PLATFORMY =======================================//
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.W && W)
                {
                    W = false; Wbutton.Enabled = false;
                    byte[] data = Encoding.ASCII.GetBytes("ARD1");
                    client.Send(data, data.Length, endpoint);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.W)
                {
                    W = true; Wbutton.Enabled = true;
                    byte[] data = Encoding.ASCII.GetBytes("ARD5");
                    client.Send(data, data.Length, endpoint);
                }
            } // W
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.S && S)
                {
                    S = false; Sbutton.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD2"); client.Send(data, data.Length, endpoint);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.S)
                {
                    S = true; Sbutton.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD6"); client.Send(data, data.Length, endpoint);
                }
            } // S
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.A && A)
                {
                    A = false; Abutton.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD3"); client.Send(data, data.Length, endpoint);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.A)
                {
                    A = true; Abutton.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD7"); client.Send(data, data.Length, endpoint);
                }
            } // A
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.D && D)
                {
                    D = false; Dbutton.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD4"); client.Send(data, data.Length, endpoint);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.D)
                {
                    D = true; Dbutton.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD8"); client.Send(data, data.Length, endpoint);
                }
            } // D
              //========================================= RUCH GŁOWY =======================================//
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.NumPad8 && _But8)
                {
                    _But8 = false; But8.Enabled = false;
                    byte[] data = Encoding.ASCII.GetBytes("ARD9");
                    client.Send(data, data.Length, endpoint);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.NumPad8)
                {
                    _But8 = true; But8.Enabled = true;
                    byte[] data = Encoding.ASCII.GetBytes("ARD13");
                    client.Send(data, data.Length, endpoint);
                }
            } // But8
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.NumPad5 && _But5)
                {
                    _But5 = false; But5.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD10"); client.Send(data, data.Length, endpoint);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.NumPad5)
                {
                    _But5 = true; But5.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD14"); client.Send(data, data.Length, endpoint);
                }
            } // But5
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.NumPad4 && _But4)
                {
                    _But4 = false; But4.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD11"); client.Send(data, data.Length, endpoint);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.NumPad4)
                {
                    _But4 = true; But4.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD15"); client.Send(data, data.Length, endpoint);
                }
            } // But4
            {
                if (m.Msg == WM_KEYDOWN && (Keys)m.WParam == Keys.NumPad6 && _But6)
                {
                    _But6 = false; But6.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD12"); client.Send(data, data.Length, endpoint);
                    return true;
                }
                else if (m.Msg == WM_KEYUP && (Keys)m.WParam == Keys.NumPad6)
                {
                    _But6 = true; But6.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD16"); client.Send(data, data.Length, endpoint);
                }
            } // But6
            return base.ProcessKeyPreview(ref m);
        }
    }
}
