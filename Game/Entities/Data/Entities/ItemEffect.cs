using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.World.Entities;

namespace Navislamia.Data.Entities;

public enum ItemEffectInstance
{
    NONE = 0,

    INC_HP = 1,
    INC_MP = 2,
    WARP = 3,
    RESURECTION = 4,
    SKILL = 5,
    ADD_STATE = 6,
    REMOVE_STATE = 7,
    TOGGLE_STATE = 8,
    ADD_IMMORAL_POINT = 41,
    SET_IMMORAL_POINT = 42,
    WARP_TO_SPECIAL_POSITION = 43,
    INC_STAMINA = 80,
    SUMMON_PET = 90,
    GENERATE_ITEM = 94,
    INC_HP_PERCENT = 101,
    INC_MP_PERCENT = 102,
    INC_GOLD = 103,
    INC_HUNTAHOLIC_POINT = 104,
    RECALL = 112,
    RESET_SKILL = 113,
    RESET_JOB = 118,
    RENAME_SUMMON = 115,
    RESET_SUMMON_SKILL = 116,
    WARP_TO_PLAYER = 117,
    RENAME_CHARACTER = 119,
    RENAME_PET = 120,
    ADD_CASH = 121,
    HAIR_DYEING = 122,
    SET_HAIR_STYLE = 123,
    ADD_STATE_EX = 124
}

public enum ItemEffectCharm
{
    NONE = 0,

    INC_MAX_STAMINA = 81,
    STAMINA_REGEN_TENT = 82,
    INC_RIDING_SPEED = 83,
    INC_SPEED = 84,
    INC_STAMINA_REGEN = 85,
    INC_MAX_WEIGHT = 86,
    INC_GAIA_MEMBERSHIP = 89,
    FAIRY_POTION = 114,
    AUTO_RECOVER_HP = 125,
    AUTO_RECOVER_MP = 126
}

public enum ItemEffectPassive
{
    NONE = 0,

    ATTACK_POINT = 11,
    MAGIC_POINT = 12,
    ACCURACY = 13,
    ATTACK_SPEED = 14,
    DEFENCE = 15,
    MAGIC_DEFENCE = 16,
    AVOID = 17,
    MOVE_SPEED = 18,
    BLOCK_CHANGE = 19,
    CARRY_WEIGHT = 20,
    BLOCK_DEFENCE = 21,
    CASTING_SPEED = 22,
    MAGIC_ACCURACY = 23,
    MAGIC_AVOID = 24,
    COOLTIME_SPEED = 25,
    BELT_SLOT = 26,
    MAX_CHAOS = 27,
    MAX_HP = 30,
    MAX_MP = 31,
    BOW_INTERVAL = 34,
    MP_REGEN_POINT = 33,
    TAMED_ITEM = 95,
    INC_PARAMETER_A = 96,
    INC_PARAMETER_B = 97,
    AMP_PARAMETER_A = 98,
    AMP_PARAMETER_B = 99
}

public enum EffectType
{
    UNKNOWN = 0,
    BASIC = 1,
    OPTIONAL = 2,
    STATE = 3,
    ENHANCE = 4
}

public class EffectInfo
{
    public const int VALUE_COUNT = 20;

    public int ID;
    public int OrdinalID;
    public EffectType Type;
    public short EffectID;
    public ushort EffectLevel;
    public VNumber[] Value = new VNumber[VALUE_COUNT];
}
