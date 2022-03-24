using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Navislamia.Network
{
    public class NetSession
    {
        public NetSession(Socket socket)
        {
            this.socket = socket;
            
        }

        Socket socket = null;
    }
}
