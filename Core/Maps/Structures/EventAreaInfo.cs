using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.X2D;

namespace Navislamia.Maps.Structures
{
    public class EventAreaInfo
    {
        public const int MaxActivateCondition = 6;

        public EventAreaInfo() 
        {
            BeginTime = 0;
            EndTime = 0;
            MinLevel = 0;
            MaxLevel = 0;
            RaceJobLimit = 0;
            LimitActivateCount = 0;
            Handler = "";

            for (int idx = 0; idx < MaxActivateCondition; ++idx)
            {
                ActivateCondition[idx] = 0;
                ActivateValue[idx][0] = 0;
                ActivateValue[idx][1] = 0;
            }
        }

        public EventAreaInfo(EventAreaInfo rh)
        {
            Area = rh.Area;
            BeginTime = rh.BeginTime;
            EndTime = rh.EndTime;
            MinLevel = rh.MinLevel;
            MaxLevel = rh.MaxLevel;
            RaceJobLimit = rh.RaceJobLimit;
            LimitActivateCount = rh.LimitActivateCount;
            Handler = rh.Handler;

            for (int idx = 0; idx < MaxActivateCondition; ++idx)
            {
                ActivateCondition[idx] = rh.ActivateCondition[idx];
                ActivateValue[idx][0] = rh.ActivateValue[idx][0];
                ActivateValue[idx][1] = rh.ActivateValue[idx][1];
            }
        }

        //TODO: IsActivatable(StructPlayer player, int areaIndex) { }

        public enum LimitCondition
        {
            NONE,
            ITEM_COUNT_GE,
            QUEST_STATUS,
            SKILL_LEVEL_GE,
            ITEM_WEARING,
            SUMMON_OWNING,
            STATE
        }

        public int BeginTime = 0;
        public int EndTime = 0;
        public int MinLevel = 0;
        public int MaxLevel = 0;

        public long RaceJobLimit = 0;
        public int[] ActivateCondition = new int[MaxActivateCondition];
        public int[][] ActivateValue = new int[MaxActivateCondition][];
        public int LimitActivateCount = 0;
        public string Handler;

        public List<PolygonF> Area = new List<PolygonF>();

        const long race_flag_gaia = 1 << 0;
        const long race_flag_deva = 1 << 1;
        const long race_flag_asura = 1 << 2;

        const long race_flag_all = (race_flag_gaia | race_flag_deva | race_flag_asura);

        const long job_flag_rogue = 1 << 3;
        const long job_flag_fighter = 1 << 4;
        const long job_flag_kahuna = 1 << 5;
        const long job_flag_spell_singer = 1 << 6;
        const long job_flag_champion = 1 << 7;
        const long job_flag_archer = 1 << 8;
        const long job_flag_druid = 1 << 9;
        const long job_flag_battle_kahuna = 1 << 10;
        const long job_flag_evoker = 1 << 11;
        const long job_flag_berserker = 1 << 12;
        const long job_flag_master_archer = 1 << 13;
        const long job_flag_high_druid = 1 << 14;
        const long job_flag_great_kahuna = 1 << 15;
        const long job_flag_beast_master = 1 << 16;
        const long job_flag_guide = 1 << 17;
        const long job_flag_holy_warrior = 1 << 18;
        const long job_flag_cleric = 1 << 19;
        const long job_flag_tamer = 1 << 20;
        const long job_flag_knight = 1 << 21;
        const long job_flag_soldier = 1 << 22;
        const long job_flag_mage = 1 << 23;
        const long job_flag_priest = 1 << 24;
        const long job_flag_breeder = 1 << 25;
        const long job_flag_crusader = 1 << 26;
        const long job_flag_blader = 1 << 27;
        const long job_flag_arch_mage = 1 << 28;
        const long job_flag_high_priest = 1 << 29;
        const long job_flag_soul_breeder = 1 << 30;
        const long job_flag_stepper = 1 << 31;
        const long job_flag_strider = 1 << 32;
        const long job_flag_magician = 1 << 33;
        const long job_flag_summoner = 1 << 34;
        const long job_flag_assassin = 1 << 35;
        const long job_flag_ranger = 1 << 36;
        const long job_flag_sorcerer = 1 << 37;
        const long job_flag_dark_magician = 1 << 38;
        const long job_flag_battle_Summoner = 1 << 39;
        const long job_flag_shadow_chaser = 1 << 40;
        const long job_flag_shadow_hunter = 1 << 41;
        const long job_flag_lich = 1 << 42;
        const long job_flag_warlock = 1 << 43;
        const long job_flag_necromancer = 1 << 44;
    }
}
