using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace UDPClient
{
    class Server
    {
        public void Listen()
        {
            UdpClient listener = new UdpClient(16010);

            IPEndPoint ServerEP = new IPEndPoint(IPAddress.Any, 16010);

            while (true)
            {
                byte[] data = listener.Receive(ref ServerEP);
                RaiseDataReceived(new ReceivedDataArgs(ServerEP.Address, ServerEP.Port, data));

            }
        }

        public delegate void DataReceived(object sender, ReceivedDataArgs args);

        public event DataReceived DataReceivedEvent;

        public void RaiseDataReceived(ReceivedDataArgs args)
        {
            if (DataReceivedEvent != null)
            {
                DataReceivedEvent(this, args);
            }
        }
    }
        public class ReceivedDataArgs
        {
            public IPAddress IpAddress { get; set; }
            public int Port { get; set; }
            public byte[] ReceivedBytes;
            public ReceivedDataArgs(IPAddress ip, int port, byte[] data)
            {
                this.IpAddress = ip;
                this.Port = port;
                this.ReceivedBytes = data;
            }
        }
    }

