using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maps.X2D;

namespace Navislamia.Data.GameContent
{
    public enum LIMIT_CONDITION
    {
        NONE,
        ITEM_COUNT_GE,
        QUEST_STATUS,
        SKILL_LEVEL_GE,
        ITEM_WEARING,
        SUMMON_OWNING,
        STATE
    }

    public static class RaceJobFlags
    {
        public const int Gaia = 1 << 0;
        public const int Deva = 1 << 1;
        public const int Asura = 1 << 2;

        public const int All = Gaia | Deva | Asura;
    }

    public static class JobFlags
    {
        // Gaia
        public const int Rogue = 1 << 0;
        public const int Fighter = 1 << 1;
        public const int Kahuna = 1 << 2;
        public const int SpellSinger = 1 << 3;
        public const int Champion = 1 << 4;
        public const int Archer = 1 << 5;
        public const int Druid = 1 << 6;
        public const int BattleKahuna = 1 << 7;
        public const int Evoker = 1 << 7;
        public const int Berserker = 1 << 8;
        public const int MasterArcher = 1 << 9;
        public const int HigherDruid = 1 << 10;
        public const int GreatKahuna = 1 << 11;
        public const int BeastMaster = 1 << 12;

        // Deva
        public const int Guide = 1 << 13;
        public const int HolyWarrior = 1 << 14;
        public const int Cleric = 1 << 15;
        public const int Tamer = 1 << 16;
        public const int Knight = 1 << 17;
        public const int Soldier = 1 << 18;
        public const int Mage = 1 << 19;
        public const int Priest = 1 << 20;
        public const int Breeder = 1 << 21;
        public const int Crusader = 1 << 22;
        public const int Blader = 1 << 23;
        public const int ArchMage = 1 << 24;
        public const int HighPriest = 1 << 25;
        public const int SoulBreeder = 1 << 26;

        // Asura
        public const int Stepper = 1 << 27;
        public const int Strider = 1 << 28;
        public const int Magician = 1 << 29;
        public const int Summoner = 1 << 30;
        public const long Assassin = 1 << 31;
        public const long Ranger = 1 << 32;
        public const long Sorcerer = 1 << 33;
        public const long DarkMagician = 1 << 34;
        public const long BattleSummoner = 1 << 35;
        public const long ShadowChaser = 1 << 36;
        public const long ShadowHunter = 1 << 37;
        public const long Lich = 1 << 38;
        public const long Warlock = 1 << 39;
        public const long Necromancer = 1 << 40;
    }

    public class EventAreaInfo
    {
        const int MaxActivateConditions = 6;

        public static bool IsActivatable(/*StructPlayer pPlayer, int areaIndex*/) => false; // TODO: implement EventAreaInfo.IsActivtable

        PolygonF area;

        int beginTime;
        int endTime;
        int minLevel;
        int maxLevel;

        long raceJobLimit;

        int[] activatieCondition = new int[MaxActivateConditions];
        int[][] activateValue = new int[MaxActivateConditions][];
        int activateMax;

        string enterHandler = string.Empty;
        string leaveHandler = string.Empty;
    }
}
