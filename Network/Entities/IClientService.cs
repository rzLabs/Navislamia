using System.Net;
using System.Net.Sockets;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Auth;
using Network;

namespace Navislamia.Network.Entities;

public interface IClientService<T>
{
    T GetEntity();
    void Create(INetworkModule networkModule, Socket socket);
    void Connect(IPEndPoint ep);
    void Disconnect();
    void Send(byte[] data);
    void Listen();
    void SendMessage(ISerializablePacket msg);
    void SendResult(ushort id, ushort result, int value = 0);
}