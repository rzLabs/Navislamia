using Navislamia.Game.Models.Arcadia;

namespace Navislamia.Game.Models.Telecaster;

public class DungeonEntity : Entity
{
    public int OwnerGuildId { get; set; }
    public virtual GuildEntity OwnerGuild { get; set; }
    
    public int RaidGuildId { get; set; }
    public GuildEntity RaidGuild { get; set; }
    
    public int BestRaidTime { get; set; }
    public int LastDungeonSiegeFinishTime { get; set; }
    public int LastDungeonRaidWrapUpTime { get; set; }
    public int TaxRate { get; set; }
}