using System;
using System.Collections.Generic;
using Navislamia.Game.Models.Arcadia;
using Navislamia.Game.Models.Enums;

namespace Navislamia.Game.Models.Telecaster;

public class GuildEntity : Entity
{
    public string Name { get; set; }
    public string Notice { get; set; }
    public string Url { get; set; }
    public string Icon { get; set; } // "game015_0000005849_100326_111217.jpg" Format - [Category]_[UniqueSID(10 characters)]_[Timestamp]_[Description].jpg
    public int IconSize { get; set; }
    public string Banner { get; set; }
    public int BannerSize { get; set; }
    public AdvertiseType AdvertiseType { get; set; }
    public DateTime AdvertiseEndTime { get; set; }
    public string AdvertiseComment { get; set; }
    public bool Recruiting { get; set; }
    public short MinRecruitLevel { get; set; }
    public short MaxRecruitLevel { get; set; }
    public bool NameChanged { get; set; }
    
    public long DungeonId { get; set; }
    public virtual DungeonEntity Dungeon { get; set; }
    
    public long DungeonBlockTime { get; set; }
    public long Gold { get; set; }
    public int Chaos { get; set; }
    
    public long AllianceId { get; set; }
    public virtual AllianceEntity Alliance { get; set; }
    
    public long AllianceBlockTime { get; set; }
    public int DonationPoint { get; set; }
    public string[] PermissionNames { get; set; } 
    public GuildPermissions[] PermissionSets { get; set; } 

    // usage could not be found in Captain
    // public long LeaderId { get; set; }
    // public virtual CharacterEntity Leader { get; set; }
    // public long RaidLeaderId { get; set; }
    // public virtual CharacterEntity RaidLeader { get; set; }
    
    public virtual ICollection<CharacterEntity> Members { get; set; }
    public virtual ICollection<CharacterEntity> PreviousMembers { get; set; }
}