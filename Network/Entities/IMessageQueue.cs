using Navislamia.Network.Enums;
using Navislamia.Network.Interfaces;
using Navislamia.Network.Packets;

namespace Navislamia.Network.Entities;

public interface IMessageQueue
{
    void Finalize(QueueType type);
    void PendSend(IClient client, ISerializablePacket msg);
    void PendReceive(IClient client, ISerializablePacket msg);
    void ProcessClientData(IClient client, byte[] data, int count);

}