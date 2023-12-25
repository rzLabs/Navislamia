using System.Collections.Generic;
using Navislamia.Game.Network.Entities;

namespace Navislamia.Game.Network.Packets.Actions;

public class GameActionState
{
    public List<ClientEntity> UnauthorizedClients { get; set; } = new();
    public List<ClientEntity> AuthorizedClients { get; set; } = new();
}