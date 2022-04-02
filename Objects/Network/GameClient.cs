using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

using Serilog;

namespace Objects.Network
{
    public class GameClient : Client
    {
        //        TcpListener listener;

        public GameClient(Socket socket, int length) : base(socket, length)
        {
        }

    }
}
