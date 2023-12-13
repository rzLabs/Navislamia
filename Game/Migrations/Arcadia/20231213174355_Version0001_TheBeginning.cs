using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Navislamia.Game.Migrations.Arcadia
{
    /// <inheritdoc />
    public partial class Version0001_TheBeginning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Left = table.Column<int>(type: "integer", nullable: false),
                    Top = table.Column<int>(type: "integer", nullable: false),
                    Right = table.Column<int>(type: "integer", nullable: false),
                    Bottom = table.Column<int>(type: "integer", nullable: false),
                    ChannelType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EffectResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EffectResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnhanceResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocalFlag = table.Column<int>(type: "integer", nullable: false),
                    EnhanceType = table.Column<int>(type: "integer", nullable: false),
                    FailResult = table.Column<int>(type: "integer", nullable: false),
                    RequiredItemId = table.Column<int>(type: "integer", nullable: false),
                    MaxEnhance = table.Column<short>(type: "smallint", nullable: false),
                    Percentage = table.Column<decimal[]>(type: "numeric[]", maxLength: 20, precision: 10, scale: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnhanceResources", x => new { x.Id, x.LocalFlag });
                });

            migrationBuilder.CreateTable(
                name: "GlobalVariables",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalVariables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemEffectResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdinalId = table.Column<long>(type: "bigint", nullable: false),
                    TooltipId = table.Column<long>(type: "bigint", nullable: false),
                    EffectId = table.Column<long>(type: "bigint", nullable: false),
                    EffectType = table.Column<int>(type: "integer", nullable: false),
                    EffectLevel = table.Column<bool>(type: "boolean", nullable: false),
                    Values = table.Column<decimal[]>(type: "numeric[]", precision: 12, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemEffectResources", x => x.Id);
                    table.CheckConstraint("CK_ItemEffectResourceEntity_Values_MaxSize20", "cardinality(\"Values\") <= 20");
                });

            migrationBuilder.CreateTable(
                name: "ItemResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemBaseType = table.Column<int>(type: "integer", nullable: false),
                    ItemType = table.Column<int>(type: "integer", nullable: false),
                    Group = table.Column<int>(type: "integer", nullable: false),
                    WearType = table.Column<int>(type: "integer", nullable: false),
                    SetPartFlag = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RaceRestriction = table.Column<int>(type: "integer", nullable: false),
                    JobRestriction = table.Column<int>(type: "integer", nullable: false),
                    ItemUseFlag = table.Column<int>(type: "integer", nullable: false),
                    DecreaseType = table.Column<byte>(type: "smallint", nullable: false),
                    Grade = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Enhance = table.Column<int>(type: "integer", nullable: false),
                    SocketCount = table.Column<int>(type: "integer", nullable: false),
                    JobDepth = table.Column<short>(type: "smallint", nullable: false),
                    UseMinLevel = table.Column<int>(type: "integer", nullable: false),
                    UseMaxLevel = table.Column<int>(type: "integer", nullable: false),
                    TargetMinLevel = table.Column<int>(type: "integer", nullable: false),
                    TargetMaxLevel = table.Column<int>(type: "integer", nullable: false),
                    Range = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    HuntaholicPoint = table.Column<int>(type: "integer", nullable: false),
                    EtherealDurability = table.Column<int>(type: "integer", nullable: false),
                    Endurance = table.Column<int>(type: "integer", nullable: false),
                    Material = table.Column<int>(type: "integer", nullable: false),
                    AvailablePeriod = table.Column<int>(type: "integer", nullable: false),
                    ThrowRange = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    BaseTypes = table.Column<short[]>(type: "smallint[]", maxLength: 4, nullable: true),
                    BaseValues = table.Column<decimal[,]>(type: "numeric[]", maxLength: 8, precision: 12, scale: 2, nullable: true),
                    OptTypes = table.Column<short[]>(type: "smallint[]", maxLength: 4, nullable: true),
                    OptValues = table.Column<decimal[,]>(type: "numeric[]", maxLength: 8, precision: 12, scale: 2, nullable: true),
                    EnhanceIds = table.Column<long?[]>(type: "bigint[]", maxLength: 2, nullable: true),
                    EnhanceValues = table.Column<decimal[,]>(type: "numeric[]", maxLength: 8, precision: 10, scale: 2, nullable: true),
                    StateLevel = table.Column<int>(type: "integer", nullable: false),
                    StateTime = table.Column<int>(type: "integer", nullable: false),
                    CoolTime = table.Column<int>(type: "integer", nullable: false),
                    CoolTimeGroup = table.Column<short>(type: "smallint", nullable: false),
                    ScriptText = table.Column<string>(type: "text", nullable: true),
                    NameId = table.Column<int>(type: "integer", nullable: true),
                    TooltipId = table.Column<int>(type: "integer", nullable: true),
                    SetId = table.Column<int>(type: "integer", nullable: true),
                    SummonId = table.Column<int>(type: "integer", nullable: true),
                    EffectId = table.Column<int>(type: "integer", nullable: true),
                    SkillId = table.Column<int>(type: "integer", nullable: true),
                    StateId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LevelResources",
                columns: table => new
                {
                    Level = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NormalExp = table.Column<long>(type: "bigint", nullable: false),
                    JLvs = table.Column<int[]>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelResources", x => x.Level);
                });

            migrationBuilder.CreateTable(
                name: "SetItemEffectResources",
                columns: table => new
                {
                    SetId = table.Column<int>(type: "integer", nullable: false),
                    SetParts = table.Column<short>(type: "smallint", nullable: false),
                    BaseTypes = table.Column<short[]>(type: "smallint[]", maxLength: 2, nullable: true),
                    BaseValues = table.Column<decimal[,]>(type: "numeric[]", maxLength: 8, precision: 12, scale: 2, nullable: true),
                    OptTypes = table.Column<short[]>(type: "smallint[]", maxLength: 2, nullable: true),
                    OptValues = table.Column<decimal[,]>(type: "numeric[]", maxLength: 8, precision: 12, scale: 2, nullable: true),
                    TextId = table.Column<int>(type: "integer", nullable: false),
                    TooltipId = table.Column<int>(type: "integer", nullable: false),
                    EffectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetItemEffectResources", x => new { x.SetId, x.SetParts });
                });

            migrationBuilder.CreateTable(
                name: "SkillResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextId = table.Column<int>(type: "integer", nullable: false),
                    DescriptionId = table.Column<int>(type: "integer", nullable: false),
                    TooltipId = table.Column<int>(type: "integer", nullable: false),
                    ElementalType = table.Column<int>(type: "integer", nullable: false),
                    IsPassive = table.Column<bool>(type: "boolean", nullable: false),
                    IsValid = table.Column<int>(type: "integer", nullable: false),
                    IsPhysicalAct = table.Column<bool>(type: "boolean", nullable: false),
                    IsHarmful = table.Column<bool>(type: "boolean", nullable: false),
                    RequiredTarget = table.Column<int>(type: "integer", nullable: false),
                    IsCorpse = table.Column<bool>(type: "boolean", nullable: false),
                    IsToggle = table.Column<bool>(type: "boolean", nullable: false),
                    IsProjectile = table.Column<bool>(type: "boolean", nullable: false),
                    ToggleGroup = table.Column<int>(type: "integer", nullable: false),
                    CastingType = table.Column<string>(type: "text", nullable: true),
                    CastingLevel = table.Column<string>(type: "text", nullable: true),
                    CastRange = table.Column<int>(type: "integer", nullable: false),
                    ValidRange = table.Column<int>(type: "integer", nullable: false),
                    CostHp = table.Column<int>(type: "integer", nullable: false),
                    CostHpPerSkl = table.Column<int>(type: "integer", nullable: false),
                    CostMp = table.Column<int>(type: "integer", nullable: false),
                    CostMpPerSkl = table.Column<int>(type: "integer", nullable: false),
                    CostMpPerEnhance = table.Column<int>(type: "integer", nullable: false),
                    CostHpPer = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CostHpPerSklPer = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CostMpPer = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CostMpPerSklPer = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CostHavoc = table.Column<int>(type: "integer", nullable: false),
                    CostHavocPerSkl = table.Column<int>(type: "integer", nullable: false),
                    CostEnergy = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CostEnergyPerSkl = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CostExp = table.Column<int>(type: "integer", nullable: false),
                    CostExpPerEnhance = table.Column<int>(type: "integer", nullable: false),
                    CostJp = table.Column<int>(type: "integer", nullable: false),
                    CostJpPerEnhance = table.Column<int>(type: "integer", nullable: false),
                    CostItem = table.Column<int>(type: "integer", nullable: false),
                    CostItemCount = table.Column<int>(type: "integer", nullable: false),
                    CostItemCountPerSkl = table.Column<int>(type: "integer", nullable: false),
                    RequriedLevel = table.Column<int>(type: "integer", nullable: false),
                    RequiredHp = table.Column<int>(type: "integer", nullable: false),
                    RequiredMp = table.Column<int>(type: "integer", nullable: false),
                    RequiredHavoc = table.Column<int>(type: "integer", nullable: false),
                    RequiredHavocBurst = table.Column<int>(type: "integer", nullable: false),
                    RequiredStateId = table.Column<int>(type: "integer", nullable: false),
                    UseWithOneHandSword = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithTwoHandSword = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithDoubleSword = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithDagger = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithDoubleDagger = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithSpear = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithAxe = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithOneHandAxe = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithDoubleAxe = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithOneHandMace = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithTwoHandMace = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithLightbow = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithHeavybow = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithCrossbow = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithOneHandStaff = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithTwoHandStaff = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithShieldOnly = table.Column<bool>(type: "boolean", nullable: false),
                    UseWithWeaponNotRequired = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnSelf = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnParty = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnGuild = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnNeutral = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnPurple = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnEnemy = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnCharacter = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnSummon = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnMonster = table.Column<bool>(type: "boolean", nullable: false),
                    DelayCast = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCastPerSkl = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCastModePerEnhance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCommon = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCooltime = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCooltimePerSkl = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCooltimeModePerEnhance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CoolTimeGroupId = table.Column<int>(type: "integer", nullable: false),
                    SkillLvupLimit = table.Column<string>(type: "text", nullable: true),
                    Target = table.Column<int>(type: "integer", nullable: false),
                    EffectType = table.Column<int>(type: "integer", nullable: false),
                    SkillEnchantLinkId = table.Column<int>(type: "integer", nullable: false),
                    StateId = table.Column<int>(type: "integer", nullable: false),
                    StateLevelBase = table.Column<int>(type: "integer", nullable: false),
                    StateLevelPerSkill = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    StateLevelPerEnhance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    StateSecond = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    StateSecondPerLevel = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    StateSecondPerEnhance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ProbabilityOnHit = table.Column<int>(type: "integer", nullable: false),
                    ProbabilityIncBySlv = table.Column<int>(type: "integer", nullable: false),
                    HitBonus = table.Column<short>(type: "smallint", nullable: false),
                    HitBonusPerEnhance = table.Column<short>(type: "smallint", nullable: false),
                    Percentage = table.Column<short>(type: "smallint", nullable: false),
                    HateMod = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    HateBasic = table.Column<short>(type: "smallint", nullable: false),
                    HatePerSkill = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    HatePerEnhance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CriticalBonus = table.Column<int>(type: "integer", nullable: false),
                    CriticalBonusPerSkl = table.Column<int>(type: "integer", nullable: false),
                    Values = table.Column<decimal[]>(type: "numeric[]", maxLength: 19, precision: 10, scale: 2, nullable: true),
                    IconId = table.Column<int>(type: "integer", nullable: false),
                    IconFileName = table.Column<string>(type: "text", nullable: true),
                    ProjectileSpeed = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ProjectileAcceleration = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StateResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextId = table.Column<int>(type: "integer", nullable: false),
                    TooltipId = table.Column<int>(type: "integer", nullable: false),
                    IsHarmful = table.Column<bool>(type: "boolean", nullable: false),
                    StateTimeType = table.Column<short>(type: "smallint", nullable: false),
                    StateGroup = table.Column<int>(type: "integer", nullable: false),
                    DuplicateGroup = table.Column<int[]>(type: "integer[]", maxLength: 3, nullable: true),
                    UseOnCharacter = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnSummon = table.Column<bool>(type: "boolean", nullable: false),
                    UseOnMonster = table.Column<bool>(type: "boolean", nullable: false),
                    ReiterationCount = table.Column<string>(type: "text", nullable: true),
                    BaseEffect = table.Column<int>(type: "integer", nullable: false),
                    FireInterval = table.Column<int>(type: "integer", nullable: false),
                    ElementalType = table.Column<int>(type: "integer", nullable: false),
                    AmplifyBase = table.Column<decimal>(type: "numeric(13,3)", precision: 13, scale: 3, nullable: false),
                    AmplifyPerSkill = table.Column<decimal>(type: "numeric(13,3)", precision: 13, scale: 3, nullable: false),
                    AddDamageBase = table.Column<int>(type: "integer", nullable: false),
                    AddDamagePerSkl = table.Column<int>(type: "integer", nullable: false),
                    EffectType = table.Column<int>(type: "integer", nullable: false),
                    Values = table.Column<decimal[]>(type: "numeric[]", maxLength: 20, precision: 13, scale: 3, nullable: true),
                    IconId = table.Column<int>(type: "integer", nullable: false),
                    IconFileName = table.Column<string>(type: "text", nullable: true),
                    FxId = table.Column<int>(type: "integer", nullable: false),
                    PosId = table.Column<int>(type: "integer", nullable: false),
                    CastSkillId = table.Column<int>(type: "integer", nullable: false),
                    CastFxId = table.Column<int>(type: "integer", nullable: false),
                    CastFxPosId = table.Column<int>(type: "integer", nullable: false),
                    HitFxId = table.Column<int>(type: "integer", nullable: false),
                    HitFxPosId = table.Column<int>(type: "integer", nullable: false),
                    SpecialOutputTimingId = table.Column<int>(type: "integer", nullable: false),
                    SpecialOutputFxId = table.Column<int>(type: "integer", nullable: false),
                    SpecialOutputFxPosId = table.Column<int>(type: "integer", nullable: false),
                    SpecialOutputFxDelay = table.Column<int>(type: "integer", nullable: false),
                    StateFxId = table.Column<int>(type: "integer", nullable: false),
                    StateFxPosId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Strength = table.Column<int>(type: "integer", nullable: false),
                    Vitality = table.Column<int>(type: "integer", nullable: false),
                    Dexterity = table.Column<int>(type: "integer", nullable: false),
                    Agility = table.Column<int>(type: "integer", nullable: false),
                    Intelligence = table.Column<int>(type: "integer", nullable: false),
                    Wisdom = table.Column<int>(type: "integer", nullable: false),
                    Luck = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StringResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SummonResources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MagicType = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Scale = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TargetFxSize = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    StandardWalkSpeed = table.Column<int>(type: "integer", nullable: false),
                    StandardRunSpeed = table.Column<int>(type: "integer", nullable: false),
                    IsRidingOnly = table.Column<bool>(type: "boolean", nullable: false),
                    RidingSpeed = table.Column<int>(type: "integer", nullable: false),
                    RunSpeed = table.Column<int>(type: "integer", nullable: false),
                    RidingMotionType = table.Column<int>(type: "integer", nullable: false),
                    AttackRange = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    WalkType = table.Column<int>(type: "integer", nullable: false),
                    SlantType = table.Column<int>(type: "integer", nullable: false),
                    Material = table.Column<int>(type: "integer", nullable: false),
                    WeaponType = table.Column<int>(type: "integer", nullable: false),
                    AttackMotionSpeed = table.Column<int>(type: "integer", nullable: false),
                    EvolveType = table.Column<int>(type: "integer", nullable: false),
                    EvolveIntoSummonId = table.Column<int>(type: "integer", nullable: false),
                    CameraPosition = table.Column<int[]>(type: "integer[]", nullable: true),
                    TargetPosition = table.Column<decimal[]>(type: "numeric[]", precision: 10, scale: 2, nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    MotionFileId = table.Column<int>(type: "integer", nullable: false),
                    FaceId = table.Column<int>(type: "integer", nullable: false),
                    FaceFileName = table.Column<string>(type: "text", nullable: true),
                    CardId = table.Column<int>(type: "integer", nullable: false),
                    ScriptText = table.Column<string>(type: "text", nullable: true),
                    IllustFileName = table.Column<string>(type: "text", nullable: true),
                    TextFeatureId = table.Column<int>(type: "integer", nullable: false),
                    SkillIds = table.Column<int[]>(type: "integer[]", nullable: true),
                    SkillTextIds = table.Column<int[]>(type: "integer[]", nullable: true),
                    StatId = table.Column<int>(type: "integer", nullable: false),
                    NameId = table.Column<int>(type: "integer", nullable: false),
                    TextureGroup = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummonResources", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelResources");

            migrationBuilder.DropTable(
                name: "EffectResources");

            migrationBuilder.DropTable(
                name: "EnhanceResources");

            migrationBuilder.DropTable(
                name: "GlobalVariables");

            migrationBuilder.DropTable(
                name: "ItemEffectResources");

            migrationBuilder.DropTable(
                name: "ItemResources");

            migrationBuilder.DropTable(
                name: "LevelResources");

            migrationBuilder.DropTable(
                name: "SetItemEffectResources");

            migrationBuilder.DropTable(
                name: "SkillResources");

            migrationBuilder.DropTable(
                name: "StateResources");

            migrationBuilder.DropTable(
                name: "StatResources");

            migrationBuilder.DropTable(
                name: "StringResources");

            migrationBuilder.DropTable(
                name: "SummonResources");
        }
    }
}
