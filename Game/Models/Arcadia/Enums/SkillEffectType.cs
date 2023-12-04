namespace DevConsole.Models.Arcadia.Enums;

public enum SkillEffectType
{
    Misc = 0,

    RespawnMonsterNearPlayer = 2,

    ParameterInc = 3,
    ParameterAmp = 4,

    RespawnMonsterRandomly = 5,
    RespawnMonsterWithDiffCode = 6, // WithDiffCode? Check usage and refactor/discard

    PhysicalSingleDamageT1 = 101,
    PhysicalMultipleDamageT1 = 102,
    PhysicalSingleDamageT2 = 103,
    PhysicalMultipleDamageT2 = 104,
    PhysicalDirectionalDamage = 105,
    PhysicalSingleDamageT3 = 106,
    PhysicalMultipleDamageT3 = 107,
    PhysicalMultipleDamageTripleAttackOld = 108,
    PhysicalSingleRegionDamageOld = 111,
    PhysicalMultipleRegionDamageOld = 112,
    PhysicalSingleSpecialRegionDamageOld = 113,
    PhysicalSingleDamageWithShield = 117,
    PhysicalAbsorbDamage = 121,
    PhysicalMultipleSpecialRegionDamageOld = 122,

    // PhysicalSingleDamageAddEnergyOld	= 125,	// was marked as deprecated check usage and refactor/discard
    // PhysicalSingleDamageKnockbackOld	= 131, // was marked as deprecated check usage and refactor/discard
    // PhysicalSingleRegionDamageKnockbackOld = 132, // was marked as deprecated check usage and refactor/discard
    PhysicalSingleDamageWithoutWeaponRushKnockBack = 151,
    // PhysicalSingleDamageRushKnockbackOld	= 152, // was marked as deprecated check usage and refactor/discard


    MagicSingleDamageT1Old = 201, // new usage 231 check usage and refactor/discard
    MagicMultipleDamageT1Old = 202, // new usage 232 check usage and refactor/discard
    MagicSingleDamageT2Old = 203, // new usage 231 check usage and refactor/discard
    MagicMultipleDamageT2Old = 204, // new usage 232 check usage and refactor/discard
    MagicMultipleDamageT3Old = 205,
    MagicMultipleDamageT1DealSummonHpOld = 206, // new usage 233 check usage and refactor/discard
    MagicSingleRegionDamageOld = 211, // new usage 261 check usage and refactor/discard
    MagicMultipleRegionDamageOld = 212, // new usage 263 check usage and refactor/discard
    MagicSpecialRegionDamageOld = 213, // new usage 262 check usage and refactor/discard
    MagicMultipleRegionDamageT2Old = 214, // new usage 263 check usage and refactor/discard
    MagicAbsorbDamageOld = 221, // new usage 235 check usage and refactor/discard

    MagicSingleDamage = 231,
    MagicMultipleDamage = 232,
    MagicMultipleDamageDealSummonHp = 233,
    MagicSingleDamageOrDeath = 234,
    MagicDamageWithAbsorbHpMp = 235,
    MagicSinglePercentDamage = 236,
    MagicSinglePercentManaburn = 237,
    MagicSinglePercentOfMaxMpManaburn = 238,
    MagicSingleDamageAddRandomState = 239,
    MagicSingleDamageByConsumingTargetsState = 240,
    MagicMultipleTargetsAtOnce = 241,

    MagicSingleRegionDamage = 261,
    MagicSpecialRegionDamage = 262,
    MagicMultipleRegionDamage = 263,
    MagicRegionPercentDamage = 264,
    MagicSingleRegionDamageUsingCorpse = 265,

    AddHpMpByAbsorbHpMp = 266,

    MagicSingleRegionDamageBySummonDead = 267,

    MagicSingleRegionDamageAddRandomState = 268,

    MagicMultipleRegionDamageAtOnce = 269,

    AreafectMagicDamage = 271,
    AreafectMagicDamageAndHeal = 272,
    AreafectMagicDamageAndHealT2 = 273,

    AddState = 301,
    AddRegionState = 302,

    CastingCancelWithAddState = 304,

    AddStateBySelfCost = 305,
    AddRegionStateBySelfCost = 306,
    AddStateByTargetType = 307,

    AddStatesWithEachDiffLv = 308,
    AddStatesWithEachDiffLvDuration = 309,
    AddStateStepByStep = 310,
    AddStateToCasterAndTarget = 311,
    AddRandomState = 312,
    AddRandomRegionState = 313,
    AddStateByUsingItem = 314,

    AreafectMagicDamageOld = 352, // new usage 271 check usage and refactor/discard
    AreafectHeal = 353,

    TrapPhysicalDamage = 381,
    TrapMagicalDamage = 382,
    TrapMultiplePhysicalDamage = 383,
    TrapMultipleMagicalDamage = 384,

    RemoveBadState = 401,
    RemoveGoodState = 402,
    AddHp = 501,
    AddMp = 502,
    Resurrection = 504,
    AddHpMp = 505,
    AddHpMpBySummonDamage = 506,
    AddHpMpBySummonDead = 507,
    AddRegionHpMp = 508,
    AddHpByItem = 509,
    AddMpByItem = 510,
    CorpseAbsorb = 511,
    AddHpMpByStealSummonHpMp = 512,
    AddHpMpWithLimitPercent = 513,

    AddRegionHp = 521,
    AddRegionMp = 522,

    Summon = 601,
    Unsummon = 602,
    UnsummonAndAddState = 605,

    ToggleAura = 701,
    ToggleDifferentialAura = 702,

    Taunt = 900,
    RegionTaunt = 901,
    RemoveHate = 902,
    RegionRemoveHate = 903,
    RegionRemoveHateOfTarget = 904,

    CorpseExplosion = 1001,

    CreateItem = 9001,
    ActivatieldProp = 9501,
    RegionHealByFieldProp = 9502,
    AreafectHealByFieldProp = 9503,

    // passives
    WeaponMastery = 10001,
    BattleParamterIncrease = 10002,
    BlockIncrease = 10003,
    AttackRangeIncrease = 10004,
    ResistanceIncrease = 10005,
    MagicRegistanceIncrease = 10006,
    SpecializeArmor = 10007,
    IncreaseBaseAttribute = 10008,
    IncreaseExtensionAttribute = 10009,

    SpecializeArmorAmp = 10010, // Armor specialization: Shield
    AmplifyBaseAttribute = 10011,

    MagicTraining = 10012,
    HuntingTraining = 10013, // Beast mastery training
    BowTraining = 10014,
    IncreaseStat = 10015,
    AmplifyStat = 10016,

    IncreaseHpMp = 10021,
    AmplifyHpMp = 10022,
    HealingAmplify = 10023,
    HealingAmplifyByItem = 10024,
    HealingAmplifyByRest = 10025, // natural healing
    HateAmplify = 10026, // Threat generation

    IncreaseSummonHpMpSp = 10031,
    AmplifySummonHpMpSp = 10032,
    BeltOnParameterInc = 10035, // Belt effect: Increase parameter stats (Strength/Agility/Intelligence)
    BeltOnAttributeInc = 10036, // Belt effect: Increase attribute stats (Fire/Earth/Water)
    BeltOnAttributeExInc = 10037, // Belt effect: Increase extended attribute stats (HP/MP, Critical Rate, Evasion)

    BeltOnAttributeEx2Inc =
        10038, // Belt effect: Increase additional extended attribute stats (Resistances, Status Effects, Secondary Attribute Points)


    UnitExpert = 10041,

    BeltOnParameterAmp = 10042, // Belt effect: Amplify parameter stats
    BeltOnAttributeAmp = 10043, // Belt effect: Amplify attribute stats
    BeltOnAttributeExAmp = 10044, // Belt effect: Amplify extended attribute stats
    BeltOnAttributeEx2Amp = 10045, // Belt effect: Amplify additional extended attribute stats


    SummonItemExpert = 10046, // Expertise in using summoning items

    AddStateOnAttack = 10048, // Add a state effect upon normal attack, subject to resistance.
    AddStateBySelfOnAttack = 10049, // Add a self-inflicted state effect upon normal attack, subject to resistance.
    AddStateOnBeingAttacked = 10050, // Add a state effect upon being attacked, subject to resistance.

    AddStateBySelfOnBeingAttacked =
        10051, // Add a self-inflicted state effect upon being attacked, subject to resistance.
    AddStateBySelfOnKill = 10052, // Add a self-inflicted state effect upon killing an enemy, subject to resistance.
    AddStateOnCriticalAttack = 10053, // Add a state effect upon critical attack, subject to resistance.

    AddStateBySelfOnCriticalAttack =
        10054, // Add a self-inflicted state effect upon critical attack, subject to resistance.

    AddStateOnBeingCriticalAttacked =
        10055, // Add a state effect upon being critically attacked, subject to resistance.

    AddStateBySelfOnBeingCriticalAttacked =
        10056, // Add a self-inflicted state effect upon being critically attacked, subject to resistance.
    AddStateOnAvoid = 10057, // Add a state effect upon successful avoidance, subject to resistance.

    AddStateBySelfOnAvoid =
        10058, // Add a self-inflicted state effect upon successful avoidance, subject to resistance.
    AddStateOnBlock = 10059, // Add a state effect upon successful block, subject to resistance.
    AddStateBySelfOnBlock = 10060, // Add a self-inflicted state effect upon successful block, subject to resistance.
    AddStateOnPerfectBlock = 10061, // Add a state effect upon successful perfect block, subject to resistance.

    AddStateBySelfOnPerfectBlock =
        10062, // Add a self-inflicted state effect upon successful perfect block, subject to resistance.
    AddEnergyOnAttack = 32262, // Add energy upon normal attack.
    AddEnergyOnBeingAttacked = 32263, // Add energy upon being attacked.
    IncSkillCoolTimeOnAttack = 10063, // Increase skill cooldown time upon normal attack.
    IncSkillCoolTimeOnBeingAttacked = 10064, // Increase skill cooldown time upon being attacked.
    IncSkillCoolTimeOnKill = 10065, // Increase skill cooldown time upon killing an enemy.
    IncSkillCoolTimeOnCriticalAttack = 10066, // Increase skill cooldown time upon critical attack.
    IncSkillCoolTimeOnBeingCriticalAttacked = 10067, // Increase skill cooldown time upon being critically attacked.
    IncSkillCoolTimeOnAvoid = 10068, // Increase skill cooldown time upon successful avoidance.
    IncSkillCoolTimeOnBlock = 10069, // Increase skill cooldown time upon successful block.
    IncSkillCoolTimeOnPerfectBlock = 10070, // Increase skill cooldown time upon successful perfect block.
    IncSkillCoolTimeOnSkillOfId = 32281, // Increase skill cooldown time on a specific skill (identified by ID).

    // 3rd 23-level skills damage and effects (Physical, Magical, or both)
    PhysicalSingleDamage = 30001, // Normal physical damage
    PhysicalSingleDamageAbsorb = 30002, // Normal physical damage with HP/MP absorption
    PhysicalSingleDamageAddEnergy = 30003, // Normal physical damage with added energy
    PhysicalSingleDamageRush = 30004, // Rushing physical damage
    PhysicalSingleDamageRushKnockback = 30005, // Rushing physical damage with knockback
    PhysicalSingleDamageKnockback = 30006, // Normal physical damage with knockback

    PhysicalSingleRegionDamageKnockback = 30007, // Region-based physical damage with knockback
    PhysicalSingleRegionDamageKnockbackSelf = 30008, // Region-based physical damage with knockback to self

    PhysicalRealtimeMultipleDamage = 30009, // Real-time multiple physical damage
    PhysicalMultipleDamageTripleAttack = 30010, // Triple attack with multiple physical damage

    PhysicalSingleRegionDamage = 30011, // Region-based single physical damage
    PhysicalMultipleRegionDamage = 30012, // Region-based multiple physical damage
    PhysicalSingleSpecialRegionDamage = 30013, // Special region-based single physical damage
    PhysicalMultipleSpecialRegionDamage = 30014, // Special region-based multiple physical damage
    PhysicalMultipleSpecialRegionDamageSelf = 30015, // Special region-based multiple physical damage to self
    PhysicalMultipleDamage = 30016, // Multiple physical damage
    PhysicalRealtimeMultipleDamageKnockback = 30017, // Real-time multiple physical damage with knockback
    PhysicalRealtimeMultipleRegionDamage = 30018, // Real-time multiple region-based physical damage
    PhysicalSingleDamageByConsumingTargetsState = 30019, // Single physical damage by consuming target's state
    PhysicalSingleRegionDamageAddingMagicalDamage = 30020, // Region-based single physical damage adding magical damage

    PhysicalSingleRegionDamageWithCastCancel = 30030, // Region-based single physical damage with cast cancellation

    ResurrectionWithRecover = 30501, // Resurrection with HP, MP, EXP recovery
    RemoveStateGroup = 30601, 
    Lotto = 30701, 

    WeaponTraining = 31001, 
    AmplifyBaseAttributeOld = 31002, 
    AmplifyExtAttribute = 31003, 

    AmplifyExpForSummon = 32001,

    EnhanceSkill = 32011, 

    MagicSingleDamageWithPhysicalDamage = 32021,

    IncreaseDamageByTargetState = 32031, 
    AmplifyDamageByTargetState = 32032, 
    TrasnferHealing = 32051, 
    PhysicalChainDamage = 32061,
    MagicChainDamage = 32062,
    ChainHeal = 32063, // Chain healing
    PhysicalSingleDamageDeminishedHpMp = 32141,
    ModifySkillCost = 32171,
    ResistHarmfulState = 32183,
    IncreaseSkillCoolTime = 32191,
    AmplifySkillCoolTime = 32192,
    IncreaseDamageAndCritRateByTargetHpRatio = 32201,
    AmplifyDamageAndCritRateByTargetHpRatio = 32202,
    AbsorbDamage = 32211, 
    StealHpMp = 32212, 
    PhysicalSingleDamageProportinalyRemainMp = 32251,
    ReplenishEnergyHpMp = 32261, // Replenish energy (added HP/MP)
    IncreaseEnergyUnconsumptionRate = 32264, 

    IncreaseParameterAmplifyHeal = 32271, 
    AmplifyParameterAmplifyHeal = 32272,

    IncreaseParamByTargetState = 32291, 
    AmplifyParamByTargetState = 32292, 

    IncParamBasedParam = 32301, // Increase parameter based on another parameter
    IncSummonParamBasedParam = 32302, // Increase summon parameter based on another parameter
    IncSummonParamBasedSummonParam = 32303, // Increase summon parameter based on another summon's parameter
    IncParamBasedSummonParam = 32304 // Increase parameter based on summon's parameter
}