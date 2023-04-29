using Navislamia.Network.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Network.Interfaces;

namespace Navislamia.Network.Packets.Actions.Interfaces
{
    public interface IAuthActionService
    {
        public int Execute(IClient client, ISerializablePacket msg);
    }
}
