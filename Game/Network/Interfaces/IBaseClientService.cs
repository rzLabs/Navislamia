using System;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Packets;

namespace Navislamia.Game.Network.Interfaces;

public interface IBaseClientService
{
    void StartClient(Guid clientId);
    void OnDataSent(int bytesSent);
    void OnDataReceived(int bytesReceived);
    void OnDataReceived(string accountName, int bytesReceived);
    void OnDisconnect();
    void OnDisconnect(string accountName);
    void SendMessage(ClientEntity client, IPacket msg);
    void AddClient(ClientEntity client);
}