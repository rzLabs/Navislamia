using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World.Repositories.Entities
{
    public class MonsterSkill
    {
        const int TriggersPerRow = 6;
        const int SkillsPerRow = 6;

        public int ID;

        public MonsterSkillTrigger[] Trigger = new MonsterSkillTrigger[TriggersPerRow];

        public MonsterSkillInfo[] Skill = new MonsterSkillInfo[SkillsPerRow];

        public MonsterSkill()
        {
            for (int i = 0; i < 6; i++)
            {
                Trigger[i] = new MonsterSkillTrigger();
                Skill[i] = new MonsterSkillInfo();
            }
        }
    }
}
