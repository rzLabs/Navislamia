using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Creature.Enums
{
    [Flags]
    public enum CreatureStatus
    {
        LoginComplete,
        FirstEnter,
        AttaackStarted,
        FirstAttack = 4,
        MovePended = 8,
        NeedUpdateState = 16,
        MovingByFear = 32,
        NeedCalculateStat = 64,
        ProcessingReflect = 128,
        Invisible = 256,
        Invincible = 512,
        Hiding = 1024, 
        Movable = 2048,
        Attackable = 4096,
        CanCastSkill = 8192,
        CanCastMagic = 16384,
        CanUseItem = 32768, 
        Mortal = 65536,
        HavocBurst = 131072,
        Feared = 262144,
        FormChanged = 524288,
        MoveSpeedFixed = 1048576,
        HpRegenStopped = 2097152,
        MpRegenStopped = 4194304, 
        UsingDoubleWeapon = 8388608,
        CompeteDead = 16777216
    }
}
