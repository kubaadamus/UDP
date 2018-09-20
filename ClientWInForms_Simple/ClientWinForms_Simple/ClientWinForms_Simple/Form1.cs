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

        UdpClient _server = null;
        IPEndPoint _client = null;
        Thread _listenThread = null;
        private bool _isServerStarted = false;
        public Form1() // 188054
        {
            InitializeComponent();

            if (_isServerStarted)
            {
                Stop();
            }
            else
            {
                Start();
            }
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
            _server = new UdpClient(serverEnd);
            //ShowMsg("Waiting for a client...");
            //Create the client end.
            _client = new IPEndPoint(IPAddress.Any, 16010);

            //Start listening.
            Thread listenThread = new Thread(new ThreadStart(Listening));
            listenThread.Start();
            //Change state to indicate the server starts.
            _isServerStarted = true;
        }

        private void Stop()
        {

                //Stop listening.
                _listenThread.Join();
                //ShowMsg("Server stops.");
                _server.Close();
                //Changet state to indicate the server stops.
                _isServerStarted = false;


        }

        private void Listening()
        {
            byte[] data;
            //Listening loop.
            while (true)
            {
                //receieve a message form a client.
                data = _server.Receive(ref _client);
                //string receivedMsg = Encoding.ASCII.GetString(data, 0, data.Length);
                //Show the message.
                this.Invoke(new ShowMessageMethod(ShowMsg),data);
                //Send a response message.
                //data = Encoding.ASCII.GetBytes("Server:" + receivedMsg);
                _server.Send(data, data.Length, _client);
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
    }
}
