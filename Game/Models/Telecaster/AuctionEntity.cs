using System;
using Navislamia.Game.Models.Arcadia;

namespace Navislamia.Game.Models.Telecaster;

public class AuctionEntity : Entity
{
    public long ItemId { get; set; }
    public virtual ItemEntity Item { get; set; }

    public long SellerId { get; set; }
    public virtual CharacterEntity Seller { get; set; }
    
    /// <summary>
    /// Might not be required since its accessible via <see cref="Seller"/>
    /// TODO check usage and remove if not required
    /// </summary>
    public string SellerName { get; set; } 
    
    public bool IsHiddenVillageOnly { get; set; } // check usage -> leftover of premium "vip" content
    public DateTime EndTime { get; set; }
    public long InstantPurchasePrice { get; set; }
    public long RegistrationTax { get; set; } // short, mid, longterm => decimal * factor 10000 -> refactor to decimal? e..g 0.18 = 18% tax 18 /100 
    public long[] BiddersIds { get; set; } // Referred from previous usage: List of character ids 
    public long HighestBiddingPrice { get; set; }
    
    public long HighestBidderId { get; set; }
    public virtual CharacterEntity HighestBidder { get; set; }

    /// <summary>
    /// Might not be required since its accessible via <see cref="HighestBidder"/>
    /// TODO check usage and remove if not required
    /// </summary>
    public string HighestBidderName { get; set; }
    
    public virtual ItemStorageEntity ItemStorage { get; set; }
}