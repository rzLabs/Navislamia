using Navislamia.Game.Models.Arcadia;

namespace Navislamia.Game.Models.Telecaster;

public class AllianceEntity : Entity
{
    public string Name { get; set; }
    
    public long LeadGuildId { get; set; }
    public virtual GuildEntity LeadGuild { get; set; }
    
    public int MaxAllianceCount { get; set; }
    public int NameChanged { get; set; }
}