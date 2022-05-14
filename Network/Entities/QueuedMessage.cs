using Navislamia.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Entities
{
    public class QueuedMessage
    {
        public Client Client;
        public ISerializablePacket Message;

        public QueuedMessage(Client client, ISerializablePacket message)
        {
            Client = client;
            Message = message;
        }
    }
}
