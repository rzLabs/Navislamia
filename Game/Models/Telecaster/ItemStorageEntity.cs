using System;
using Navislamia.Game.Models.Arcadia;
using Navislamia.Game.Models.Enums;

namespace Navislamia.Game.Models.Telecaster;

/// <summary>
/// Formerly "ItemKeeping"
/// </summary>
public class ItemStorageEntity : Entity
{
    public long ItemId { get; set; }
    public virtual ItemEntity Item { get; set; }
    
    public int CharacterId { get; set; } // at a alter point refactor this to use account id for global storage across all characters?
    public virtual CharacterEntity Character { get; set; }

    public DateTime ExpirationTime { get; set; }
    public StorageType StorageType { get; set; }
    public int RelatedAuctionId { get; set; }
    public virtual AuctionEntity RelatedAuction { get; set; }
    public long RelatedItemId { get; set; }
    public int RelatedItemEnhance { get; set; }
    public int RelatedItemLevel { get; set; }
}