using Navislamia.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Interfaces
{
    public interface IClient
    {
        public int Connect(IPEndPoint ep);

        public void Send(Packet msg, bool beginReceive = true);
    }
}
