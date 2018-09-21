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

namespace ClientWinForms_Simple
{
    public partial class Form1 : Form
    {
        delegate void ShowMessageMethod(byte[] msg);
        UdpClient client = null;
        IPEndPoint endpoint = null;
        Thread listenThread = null;
        private bool isServerStarted = false;
        bool W = true;
        bool S = true;
        bool A = true;
        bool D = true;
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
            IPEndPoint serverEnd = new IPEndPoint(IPAddress.Any, 16010);
            client = new UdpClient(serverEnd);
            //ShowMsg("Waiting for a client...");
            //Create the client end.
            endpoint = new IPEndPoint(IPAddress.Any, 16010);

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
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W && W) { W = false; Wbutton.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD1"); client.Send(data, data.Length, endpoint); }
            if (e.KeyCode == Keys.S && S) { S = false; Sbutton.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD2"); client.Send(data, data.Length, endpoint); }
            if (e.KeyCode == Keys.A && A) { A = false; Abutton.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD3"); client.Send(data, data.Length, endpoint); }
            if (e.KeyCode == Keys.D && D) { D = false; Dbutton.Enabled = false; byte[] data = Encoding.ASCII.GetBytes("ARD4"); client.Send(data, data.Length, endpoint); }
            textBox1.Clear();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { W = true; Wbutton.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD5"); client.Send(data, data.Length, endpoint); }
            if (e.KeyCode == Keys.S) { S = true; Sbutton.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD6"); client.Send(data, data.Length, endpoint); }
            if (e.KeyCode == Keys.A) { A = true; Abutton.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD7"); client.Send(data, data.Length, endpoint); }
            if (e.KeyCode == Keys.D) { D = true; Dbutton.Enabled = true; byte[] data = Encoding.ASCII.GetBytes("ARD8"); client.Send(data, data.Length, endpoint); }
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


    }
}
