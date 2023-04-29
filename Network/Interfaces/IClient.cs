using Navislamia.Network.Entities;
using Navislamia.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Interfaces
{
    public interface IClient
    {
        public ClientEntity GetEntity();

        public void PendMessage(ISerializablePacket msg);

        public void Send(byte[] data);
    }
}
