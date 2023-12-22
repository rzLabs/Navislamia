using Navislamia.Game.Network.Entities;
using Navislamia.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Actions
{
    public interface IAuthActionService
    {
        void Execute(AuthClientService client, IPacket msg);
    }

    public interface IUploadActionService
    {
        void Execute(UploadClientService client, IPacket msg);
    }

    public interface IGameActionService
    {
        void Execute(GameClientService client, IPacket msg);
    }

}
