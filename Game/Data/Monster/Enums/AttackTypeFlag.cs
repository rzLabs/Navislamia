using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data.Monster
{
    public enum AttackTypeFlag
    {
        FirstAttack = 1 << 0,
        GroupFirstAttack = 1 << 1,
        ResponseCasting = 1 << 2,
        ResponseRace = 1 << 3,
        ResponseBattle = 1 << 4
    }
}
