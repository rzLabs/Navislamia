using System;

namespace Navislamia.Game.Maps.Enums;
[Flags]
public enum Jobs
{
    // Gaia
    Rogue = 1 << 0,
    Fighter = 1 << 1,
    Kahuna = 1 << 2,
    SpellSinger = 1 << 3,
    Champion = 1 << 4,
    Archer = 1 << 5,
    Druid = 1 << 6,
    BattleKahuna = 1 << 7,
    Evoker = 1 << 7,
    Berserker = 1 << 8,
    MasterArcher = 1 << 9,
    HigherDruid = 1 << 10,
    GreatKahuna = 1 << 11,
    BeastMaster = 1 << 12,

    // Deva
    Guide = 1 << 13,
    HolyWarrior = 1 << 14,
    Cleric = 1 << 15,
    Tamer = 1 << 16,
    Knight = 1 << 17,
    Soldier = 1 << 18,
    Mage = 1 << 19,
    Priest = 1 << 20,
    Breeder = 1 << 21,
    Crusader = 1 << 22,
    Blader = 1 << 23,
    ArchMage = 1 << 24,
    HighPriest = 1 << 25,
    SoulBreeder = 1 << 26,

    // Asura
    Stepper = 1 << 27,
    Strider = 1 << 28,
    Magician = 1 << 29,
    Summoner = 1 << 30,
    Assassin = 1 << 31,
    Ranger = 1 << 32,
    Sorcerer = 1 << 33,
    DarkMagician = 1 << 34,
    BattleSummoner = 1 << 35,
    ShadowChaser = 1 << 36,
    ShadowHunter = 1 << 37,
    Lich = 1 << 38,
    Warlock = 1 << 39,
    Necromancer = 1 << 40,
}