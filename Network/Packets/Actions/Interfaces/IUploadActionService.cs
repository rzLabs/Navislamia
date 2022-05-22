using Navislamia.Network.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Actions.Interfaces
{
    public interface IUploadActionService
    {
        public int Execute(Client client, ISerializablePacket msg);
    }
}
