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
            //Console.WriteLine("mam!" + msg[0]);
            pictureBox1.Image = byteArrayToImage(msg);
        }
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            ImageConverter imageConverter = new System.Drawing.ImageConverter();
            Image image = imageConverter.ConvertFrom(byteArrayIn) as Image;

            return image;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes("Dostalem!!!!!");
            client.Send(data, data.Length, endpoint);
        }
    }
}
