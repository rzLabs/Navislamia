namespace Navislamia.Game.Network.Entities;

public class GameClientEntity : ClientEntity
{
    public ConnectionInfo Info { get; set; } = new ConnectionInfo();
}