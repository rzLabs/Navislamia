using Navislamia.Game.Network.Entities.Actions;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets;
using Navislamia.Game.Network.Packets.Enums;
using Serilog;
using System;

namespace Navislamia.Game.Network.Entities;

public class Client : IDisposable
{
    private readonly ILogger _logger = Log.ForContext<Client>();

    private readonly NetworkService _networkService;

    internal readonly IActions Actions;

    public ClientType Type { get; set; }

    public Connection Connection { get; set; }

    internal ConnectionInfo ConnectionInfo { get; set; } = new();

    public string ClientTag
    {
        get
        {
            var clientTag = $"{Enum.GetName(typeof(ClientType), Type)} ";

            switch (Type)
            {
                case ClientType.Auth:
                case ClientType.Upload:
                    clientTag += "Server ";
                    break;

                case ClientType.Game:
                    clientTag += "Client ";
                    break;
            }

            clientTag += $"@{Connection.RemoteIp}:{Connection.RemotePort}";

            return clientTag;
        }
    }

    public Client(NetworkService networkService, ClientType type)
    {
        _networkService = networkService;

        Type = type;

        Actions = Type switch
        {
            ClientType.Auth => new AuthActions(networkService),
            ClientType.Upload => new UploadActions(networkService),
            ClientType.Game => new GameActions(networkService),

            _ => throw new Exception("Type not associated with any actions!")
        };
    }

    public virtual void OnDataSent(int bytesSent) { }

    public virtual void OnDisconnect() 
    {
        switch (Type) 
        {
            case ClientType.Auth:
            case ClientType.Upload:
                // TODO: handle auth disconnection
                break;

            case ClientType.Game:
                {
                    _networkService.AuthorizedGameClients.Remove(ConnectionInfo.AccountName);

                    var logoutMsg = new Packet<TS_GA_CLIENT_LOGOUT>((ushort)AuthPackets.TS_GA_CLIENT_LOGOUT, new TS_GA_CLIENT_LOGOUT(ConnectionInfo.AccountName, (uint)ConnectionInfo.ContinuousPlayTime));

                    _networkService.AuthClient.SendMessage(logoutMsg);

                    Dispose();
                }
                break;
        }
    }

    public virtual void OnDataReceived(int bytesReceived) { }

    public virtual void SendMessage(IPacket msg)
    {
        Connection.Send(msg.Data);
    }

    public void Dispose()
    {
        Connection.Disconnect();
        Connection = null;
        ConnectionInfo = null;
    }
}