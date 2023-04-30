using System.Net;
using System.Net.Sockets;
using Navislamia.Network.Packets;

namespace Navislamia.Network.Entities;

public interface IClientService<T>
{
    T GetEntity();
    void Create(Socket socket);
    int Connect(IPEndPoint ep);
    void Send(byte[] data);
    void Listen();
    void PendMessage(ISerializablePacket msg);
    void SendResult(ushort id, ushort result, int value = 0);
}