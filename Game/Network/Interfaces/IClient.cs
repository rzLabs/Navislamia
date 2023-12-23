using Navislamia.Game.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network
{
    public interface IClient
    {
        void Start();

        void SendMessage(IPacket msg);
    }
}
