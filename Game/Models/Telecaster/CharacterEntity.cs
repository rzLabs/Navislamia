using System;
using System.Collections.Generic;
using Navislamia.Game.Models.Enums;

namespace Navislamia.Game.Models.Telecaster;

public class CharacterEntity : Entity
{
	public string CharacterName { get; set; }
	public string AccountName { get; set; }	
	    
	// relation not possible since you can't create a relation across contexts
	// (TelecasterContext => AuthContext)
	public long AccountId { get; set; }
	
	public long? PartyId { get; set; }
	public virtual PartyEntity Party { get; set; }
	
	public long? GuildId { get; set; }
	public virtual GuildEntity Guild { get; set; }
	
	public long? PreviousGuildId { get; set; }
	public virtual GuildEntity PreviousGuild { get; set; }

	public int Slot { get; set; }
	public int Permission { get; set; }
	public int[] Position { get; set; } // X Y Z 
	public int Layer { get; set; }
	public int Race { get; set; }
	public int Sex { get; set; }
	public int Lv { get; set; }
	public int MaxReachedLv { get; set; }
	public long Exp { get; set; }
	public long LastDecreasedExp { get; set; }
	public int Hp { get; set; }
	public int Mp { get; set; }
	public int Stamina { get; set; }
	public int Havoc { get; set; } // check usage and remove if unused
	public Job CurrentJob { get; set; }
	public Job[] PreviousJobs { get; set; } 
	public JobDepth JobDepth { get; set; }
	public int Jlv { get; set; }
	public long Jp { get; set; }
	public long TotalJp { get; set; }
	public int TalentPoint { get; set; }
	public int[] JobLvs { get; set; } 
	public decimal ImmoralPoint { get; set; }
	public int Charisma { get; set; } // used or not?
	public int PkCount { get; set; } 
	public int DkCount { get; set; }
	public int HuntaholicPoint { get; set; }
	public int HuntaholicEnterCount { get; set; }
	public int EtherealStoneDurability { get; set; }
	public long Gold { get; set; }
	public int Chaos { get; set; }
	public int SkinColor { get; set; }
	public int[] Models { get; set; } 
	public int HairColorIndex { get; set; }
	public int HairColorRgb { get; set; }
	public int HideEquipFlag { get; set; }
	public int TextureId { get; set; }
	public long[] BeltItemIds { get; set; } // verify if item id or item resource id and create reference & navigational prop
	public long[] SummonSlotItemIds { get; set; } // verify if item id or item resource id and create reference & navigational prop
	
	public long? MainSummonId { get; set; }
	public virtual SummonEntity MainSummon { get; set; }

	public long? SubSummonId { get; set; }
	public virtual SummonEntity SubSummon { get; set; }
	
	public int RemainSummonTime { get; set; }
	
	public long? PetId { get; set; } 
	public virtual PetEntity Pet { get; set; }
	
	public DateTime? LoginTime { get; set; }
	public int LoginCount { get; set; }
	public DateTime? LogoutTime { get; set; }
	public int PlayTime { get; set; } // probably unused -> leave for new feature e.g. show account playtime in char info?
	public int ChatBlockTime { get; set; } // refactor to DateTime -> BlockedUntil <timestamp>
	public int AdvChatCount { get; set; }
	public bool WasNameChanged { get; set; }
	public bool AutoUsed { get; set; }
	public DateTime? GuildBlockTime { get; set; }
	public bool PkMode { get; set; }
	public int OtpValue { get; set; } // otp = one time password
	public DateTime? OtpVerifiedAt { get; set; }
	public string[] FlagList { get; set; } // Lua stuff e.g.ry:49481...
	public string[] ClientInfo { get; set; } // BaseClientService stuff e.g. KGM=02,1,3|AKA=2,513,3...
	
	public virtual ICollection<ItemEntity> Items { get; set; }
	public virtual ICollection<AuctionEntity> Sellers { get; set; }
	public virtual ICollection<AuctionEntity> HighestBidders { get; set; }
	public virtual ItemStorageEntity ItemStorage { get; set; }
	public virtual PartyEntity LeadersParty { get; set; }

}