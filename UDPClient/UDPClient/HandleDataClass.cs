using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPClient
{
    class HandleDataClass
    {
        int count = 0;
        public void SubscribeToEvent(Server server)
        {
            server.DataReceivedEvent += server_DataReceivedEvent;
        }

        void server_DataReceivedEvent(object sender, ReceivedDataArgs args)
        {
            Console.WriteLine(count++);
        }
    }
}
