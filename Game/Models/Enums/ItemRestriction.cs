using System;

namespace Navislamia.Game.Models.Arcadia.Enums
{
    [Flags]
    public enum ItemRestriction : short
    {
        Deva = 1,
        Asura = 2,
        Gaia = 4,
        
        Fighter = 1024,
        Hunter = 2048,
        Magician = 4096,
        Summoner = 8192
    }
}