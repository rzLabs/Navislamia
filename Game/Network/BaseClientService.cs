using System;
using System.Collections.Generic;
using System.Linq;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Entities;

using Serilog;

namespace Navislamia.Game.Network;

public class BaseClientService : IBaseClientService
{
    private readonly ILogger _logger = Log.Logger.ForContext<BaseClientService>();
    protected readonly List<ClientEntity> Clients = new();

    public void StartClient(Guid clientId)
    {
        var client = Clients.FirstOrDefault(c => c.Id == clientId);
        if (client == null)
        {
            throw new Exception("Could not start client. Client could not be found");
        }
        client.Connection.OnDataSent = OnDataSent;
        client.Connection.OnDataReceived = OnDataReceived;
        client.Connection.OnDisconnected = OnDisconnect;

        client.Connection.Start();
    }

    public void AddClient(ClientEntity client)
    {
        Clients.Add(client);
    }

    public virtual void OnDataSent(int bytesSent) { }

    public virtual void OnDataReceived(string accountName, int bytesReceived) { }

    public virtual void OnDisconnect() { }

    public virtual void OnDataReceived(int bytesReceived) { }

    public virtual void OnDisconnect(string accountName) { }

    public virtual void SendMessage(ClientEntity client, IPacket msg)
    {
        client.Connection.Send(msg.Data);
        _logger.Debug("{name} ({id}) Length: {length} sent to {clientTag}", 
            msg.StructName, msg.ID, msg.Length, client.Type);
    }
    
}