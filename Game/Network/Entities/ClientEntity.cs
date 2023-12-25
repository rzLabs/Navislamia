using System;
using Navislamia.Game.Network.Packets.Enums;

namespace Navislamia.Game.Network.Entities;

public class ClientEntity
{
    public Guid Id { get; set; } // used by auth and upload 
    public ClientType Type { get; set; }
    public string AccountName { get; set; } // used by auth to identify client
    public uint PacketVersion { get; set; } = 0;
    public ClientDetails Details { get; set; } = new();
    public Connection Connection { get; set; }
    public ClientData ConnectionData { get; set; }
    public string ClientTag { get; set; }
    public bool IsAuthorized { get; set; } = false;
}