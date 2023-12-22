namespace Navislamia.Game.Models.Enums;

public enum StorageType
{
    Unknown = 0,

    ItemBySuccessfulBid = 1,
    ItemByInstantPurchase = 2,
    ItemByExpiration = 3,
    ItemByCancel = 4,

    GoldByItemSell = 30,
    GoldByRegTax = 31,
    GoldByHigherBid = 32,
    GoldByCancel = 33,
    GoldByItemSoldOut = 34,
}