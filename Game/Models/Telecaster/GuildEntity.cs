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
    
    public int DungeonId { get; set; }
    public virtual DungeonEntity Dungeon { get; set; }
    
    public long DungeonBlockTime { get; set; }
    public long Gold { get; set; }
    public int Chaos { get; set; }
    
    public int AllianceId { get; set; }
    public virtual AllianceEntity Alliance { get; set; }
    
    public long AllianceBlockTime { get; set; }
    public int DonationPoint { get; set; }
    public string PermissionName1 { get; set; }
    public int PermissionSet1 { get; set; }
    public string PermissionName2 { get; set; }
    public int PermissionSet2 { get; set; }
    public string PermissionName3 { get; set; }
    public int PermissionSet3 { get; set; }
    public string PermissionName4 { get; set; }
    public int PermissionSet4 { get; set; }
    public string PermissionName5 { get; set; }
    public int PermissionSet5 { get; set; }
    public string PermissionName6 { get; set; }
    public int PermissionSet6 { get; set; }
    
    public int LeaderId { get; set; }
    public virtual CharacterEntity Leader { get; set; }
    
    public int RaidLeaderId { get; set; }
    public virtual CharacterEntity RaidLeader { get; set; }
    
    public virtual ICollection<CharacterEntity> Members { get; set; }
    public virtual ICollection<CharacterEntity> PreviousMembers { get; set; }
}