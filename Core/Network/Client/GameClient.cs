using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

using Navislamia.Configuration;
using Navislamia.Network.Security;

using Serilog;

namespace Navislamia.Network
{
    public class GameClient
    {
        ConfigurationManager configMgr = ConfigurationManager.Instance;

        public GameClient(Socket socket, int bufferLength)
        {
            Socket = socket;
            Data = new byte[bufferLength];
        }

        public int DataLength => Data?.Length ?? -1;

        public byte[] Data = null;

        public Socket Socket = null;
    }
}
