using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Packets.Enums
{
    public enum DisconnectType
    {
        AnotherLogin = 0,
        DuplicatedLogin = 1,
        BillingExpired = 2,
        GameAddiction = 3,
        DbError = 100,
        AntiHack = 101,
        Script = 102
    }
}
