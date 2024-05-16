using System.Collections.Generic;

using Navislamia.Game.Creature;

namespace Navislamia.Game.Network.Clients;

public class ConnectionInfo
{
    public string AccountName { get; set; }
    public List<string> CharacterList { get; set; } = new();

    public Player Player { get; set; } = null;

    public int AccountId { get; set; }
    public int Version { get; set; }
    public float LastReadTime { get; set; }
    public bool AuthVerified { get; set; }
    public byte PcBangMode { get; set; }
    public int EventCode { get; set; }
    public int Age { get; set; }
    public int AgeLimitFlags { get; set; }
    public float ContinuousPlayTime { get; set; }
    public float ContinuousLogoutTime { get; set; }
    public float LastContinuousPlayTimeProcTime;
    public string NameToDelete { get; set; }
    public bool StorageSecurityCheck { get; set; } = false;
}