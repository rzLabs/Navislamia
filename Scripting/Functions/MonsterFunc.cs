using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting.Functions
{
    public static class MonsterFunc
    {
        public static int get_monster_id(params object[] args)
        {
            //    int n = lua_gettop(L);          // number of arguments

            //    if (n < 1 || !lua_isnumber(L, 1))
            //    {
            //        lua_pushnumber(L, 0);
            //        LUA()->Log("SCRIPT_GetMonsterID() : invalid argument");
            //        return 1;
            //    }

            //    AR_HANDLE hMonster = lua_tonumber(L, 1);

            //    StructMonster::iterator it = StructMonster::get(hMonster);

            return 1;
        }
    }
}
