using Navislamia.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Database.Entities
{
    public class MonsterSkillTrigger
    {
        public int ID;

        public float[] Value { get; set; } = new float[2];

        public string Script;
    }

    public class MonsterSkillInfo
    {
        public int ID;
        public int LV;
        public float Probability;
    }

    public class MonsterSkill
    {
        const int TriggersPerRow = 6;
        const int SkillsPerRow = 6;
        const int ItemDropsPerRow = 10;

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
