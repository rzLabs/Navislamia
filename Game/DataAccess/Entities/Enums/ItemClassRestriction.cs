using System;

namespace Navislamia.Game.DataAccess.Entities.Enums
{
    [Flags]
    public enum ItemClassRestriction : short
    {
        LimitDeva = 4,
        LimitAsura = 8,
        LimitGaia = 16,
    }
}
