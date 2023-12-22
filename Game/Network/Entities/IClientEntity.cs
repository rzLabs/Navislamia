using Navislamia.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Entities
{
    public interface IClientEntity
    {
        IConnection Connection { get; }

        string ClientTag { get; }

        void SendMessage(IPacket msg);
    }
}
