using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Database.Entities
{
    public struct MonsterItemDropInfo
    {
        public int ItemID;
        public float Percentage;
        public short MinCount, MaxCount;
        public short MinLevel, MaxLevel;
    }
}
