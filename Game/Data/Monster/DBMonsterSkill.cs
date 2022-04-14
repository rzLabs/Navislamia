using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data.Monster
{
    public class DBMonsterSkill
    {
        public int ID;
        public MonsterTrigger[] Trigger;
        public MonsterSkillInfo[] SkillInfo;
        public decimal[][] DV_Trigger;
        public decimal[] DV_SkillProbability;
    }
}
