using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data.Monster
{
    public struct MonsterItemDropInfo
    {
        public int ItemID;
        public int Percentage;
        public short MinCount, MaxCount;
        public short MinLevel, MaxLevel;
    }
}
