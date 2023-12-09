using System;
using System.Collections.Generic;
using Navislamia.Game.Models.Arcadia;
using Navislamia.Game.Models.Enums;

namespace Navislamia.Game.Models.Telecaster;

public class CharacterEntity : Entity
{
	public string CharacterName { get; set; }
	public string AccountName { get; set; }	
	    
	// relation not possible since you can't create a relation across contexts
	// (TelecasterContext => AuthContext)
	public int AccountId { get; set; }
	
	public int PartyId { get; set; }
	public virtual PartyEntity Party { get; set; }
	
	public int GuildId { get; set; }
	public virtual GuildEntity Guild { get; set; }
	
	public int PreviousGuildId { get; set; }
	public virtual GuildEntity PreviousGuild { get; set; }

	public int Slot { get; set; }
	public int Permission { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	public int Layer { get; set; }
	public int Race { get; set; }
	public int Sex { get; set; }
	public int Lv { get; set; }
	public int MaxReachedLevel { get; set; }
	public long Exp { get; set; }
	public long LastDecreasedExp { get; set; }
	public int Hp { get; set; }
	public int Mp { get; set; }
	public int Stamina { get; set; }
	public int Havoc { get; set; }
	public int Job { get; set; }
	public JobDepth JobDepth { get; set; }
	public int Jlv { get; set; }
	public long Jp { get; set; }
	public long TotalJp { get; set; }
	public int TalentPoint { get; set; }
	public int Job0 { get; set; }
	public int Job1 { get; set; }
	public int Job2 { get; set; }
	public int Jlv0 { get; set; }
	public int Jlv1 { get; set; }
	public int Jlv2 { get; set; }
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
	public int Model00 { get; set; }
	public int Model01 { get; set; }
	public int Model02 { get; set; }
	public int Model03 { get; set; }
	public int Model04 { get; set; }
	public int HairColorIndex { get; set; }
	public int HairColorRgb { get; set; }
	public int HideEquipFlag { get; set; }
	public int TextureId { get; set; }
	public long Belt00 { get; set; }
	public long Belt01 { get; set; }
	public long Belt02 { get; set; }
	public long Belt03 { get; set; }
	public long Belt04 { get; set; }
	public long Belt05 { get; set; }
	public int SummonSlot0 { get; set; }
	public int SummonSlot1 { get; set; }
	public int SummonSlot2 { get; set; }
	public int SummonSlot3 { get; set; }
	public int SummonSlot4 { get; set; }
	public int SummonSlot5 { get; set; }
	
	public int MainSummonId { get; set; }
	public virtual SummonEntity MainSummon { get; set; }

	public int SubSummonId { get; set; }
	public virtual SummonEntity SubSummon { get; set; }
	
	public int RemainSummonTime { get; set; }
	public int PetId { get; set; } 
	public virtual PetEntity Pet { get; set; }
	public DateTime CreatedOn { get; set; } // TODO introduce CreatedOn into DbContext as part of softdeletion @Nexitis
	public DateTime DeletedOn { get; set; } // TODO introduce DeletedOn into DbContext as part of softdeletion @Nexitis
	public DateTime LoginTime { get; set; }
	public int LoginCount { get; set; }
	public DateTime LogoutTime { get; set; }
	public int PlayTime { get; set; }
	public int ChatBlockTime { get; set; }
	public int AdvChatCount { get; set; }
	public int NameChanged { get; set; }
	public int AutoUsed { get; set; }
	public DateTime GuildBlockTime { get; set; }
	public bool PkMode { get; set; }
	public int OtpValue { get; set; } // Time related configuration
	public DateTime OtpDate { get; set; } // Time related configuration expiration time
	public string FlagList { get; set; } 
	public ClientInfo[] ClientInfo { get; set; } // join values with , seperator and join clientInfos with | seperator
	
	public virtual ICollection<ItemEntity> Items { get; set; }
	public virtual ICollection<AuctionEntity> Auctions { get; set; }
	public virtual ItemStorageEntity ItemStorage { get; set; }
}