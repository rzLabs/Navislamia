using System.Collections.Generic;
using Navislamia.Game.Network.Entities;

namespace Navislamia.Game.Network.Packets.Actions;

public class GameActionState
{
    public List<Client> UnauthorizedClients { get; set; } = new();
    public List<Client> AuthorizedClients { get; set; } = new();
}