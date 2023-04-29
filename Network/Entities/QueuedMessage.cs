using Navislamia.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Network.Interfaces;

namespace Navislamia.Network.Entities
{
    public class QueuedMessage
    {
        public IClient Client;
        public ISerializablePacket Message;

        public QueuedMessage(IClient client, ISerializablePacket message)
        {
            Client = client;
            Message = message;
        }
    }
}
