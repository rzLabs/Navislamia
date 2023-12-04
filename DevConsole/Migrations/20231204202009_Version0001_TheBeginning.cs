using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DevConsole.Migrations
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
                    Id = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnhanceType = table.Column<int>(type: "integer", nullable: false),
                    FailResult = table.Column<int>(type: "integer", nullable: false),
                    LocalFlag = table.Column<int>(type: "integer", nullable: false),
                    NeedItem = table.Column<int>(type: "integer", nullable: false),
                    MaxEnhance = table.Column<short>(type: "smallint", nullable: false),
                    Percentage = table.Column<decimal[]>(type: "numeric[]", maxLength: 20, precision: 10, scale: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnhanceResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalVariables",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalVariables", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ItemEffectResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrdinalId = table.Column<int>(type: "integer", nullable: false),
                    EffectId = table.Column<short>(type: "smallint", nullable: false),
                    EffectType = table.Column<int>(type: "integer", nullable: false),
                    EffectLevel = table.Column<short>(type: "smallint", nullable: false),
                    Value = table.Column<int[]>(type: "integer[]", nullable: true),
                    TooltipId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemEffectResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemBaseType = table.Column<int>(type: "integer", nullable: false),
                    ItemType = table.Column<int>(type: "integer", nullable: false),
                    Group = table.Column<int>(type: "integer", nullable: false),
                    WearType = table.Column<int>(type: "integer", nullable: false),
                    SetPartFlag = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ClassRestriction = table.Column<short>(type: "smallint", nullable: false),
                    JobRestriction = table.Column<short>(type: "smallint", nullable: false),
                    ItemUseFlag = table.Column<int>(type: "integer", nullable: false),
                    DecreaseType = table.Column<int>(type: "integer", nullable: false),
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
                    ThrowRange = table.Column<decimal>(type: "numeric", nullable: false),
                    BaseType = table.Column<int[]>(type: "integer[]", maxLength: 2, nullable: true),
                    BaseVar = table.Column<decimal[,]>(type: "numeric[]", maxLength: 8, precision: 12, scale: 2, nullable: true),
                    OptType = table.Column<int[]>(type: "integer[]", maxLength: 2, nullable: true),
                    OptVar = table.Column<decimal[,]>(type: "numeric[]", maxLength: 8, precision: 12, scale: 2, nullable: true),
                    EnchanceId = table.Column<int[]>(type: "integer[]", maxLength: 2, nullable: true),
                    EnchanceVar = table.Column<decimal[,]>(type: "numeric[]", maxLength: 8, precision: 10, scale: 2, nullable: true),
                    StateLevel = table.Column<int>(type: "integer", nullable: false),
                    StateTime = table.Column<int>(type: "integer", nullable: false),
                    CoolTime = table.Column<int>(type: "integer", nullable: false),
                    CoolTimeGroup = table.Column<short>(type: "smallint", nullable: false),
                    ScriptText = table.Column<string>(type: "text", nullable: true),
                    NameId = table.Column<int>(type: "integer", nullable: false),
                    TooltipId = table.Column<int>(type: "integer", nullable: false),
                    SetId = table.Column<int>(type: "integer", nullable: false),
                    SummonId = table.Column<int>(type: "integer", nullable: false),
                    EffectId = table.Column<int>(type: "integer", nullable: false),
                    SkillId = table.Column<int>(type: "integer", nullable: false),
                    StateId = table.Column<int>(type: "integer", nullable: false)
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
                    Jl = table.Column<int[]>(type: "integer[]", nullable: true)
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
                    BaseType = table.Column<int[]>(type: "integer[]", nullable: true),
                    BaseVar = table.Column<decimal[,]>(type: "numeric[]", nullable: true),
                    OptType = table.Column<int[]>(type: "integer[]", nullable: true),
                    OptVar = table.Column<decimal[,]>(type: "numeric[]", nullable: true),
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextId = table.Column<int>(type: "integer", nullable: false),
                    DescId = table.Column<int>(type: "integer", nullable: false),
                    TooltipId = table.Column<int>(type: "integer", nullable: false),
                    Elemental = table.Column<string>(type: "text", nullable: true),
                    IsPassive = table.Column<string>(type: "text", nullable: true),
                    IsPhysicalAct = table.Column<string>(type: "text", nullable: true),
                    IsHarmful = table.Column<string>(type: "text", nullable: true),
                    IsNeedTarget = table.Column<string>(type: "text", nullable: true),
                    IsCorpse = table.Column<string>(type: "text", nullable: true),
                    IsToggle = table.Column<string>(type: "text", nullable: true),
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
                    NeedLevel = table.Column<int>(type: "integer", nullable: false),
                    NeedHp = table.Column<int>(type: "integer", nullable: false),
                    NeedMp = table.Column<int>(type: "integer", nullable: false),
                    NeedHavoc = table.Column<int>(type: "integer", nullable: false),
                    NeedHavocBurst = table.Column<int>(type: "integer", nullable: false),
                    NeedStateId = table.Column<int>(type: "integer", nullable: false),
                    VfOneHandSword = table.Column<string>(type: "text", nullable: true),
                    VfTwoHandSword = table.Column<string>(type: "text", nullable: true),
                    VfDoubleSword = table.Column<string>(type: "text", nullable: true),
                    VfDagger = table.Column<string>(type: "text", nullable: true),
                    VfDoubleDagger = table.Column<string>(type: "text", nullable: true),
                    VfSpear = table.Column<string>(type: "text", nullable: true),
                    VfAxe = table.Column<string>(type: "text", nullable: true),
                    VfOneHandAxe = table.Column<string>(type: "text", nullable: true),
                    VfDoubleAxe = table.Column<string>(type: "text", nullable: true),
                    VfOneHandMace = table.Column<string>(type: "text", nullable: true),
                    VfTwoHandMace = table.Column<string>(type: "text", nullable: true),
                    VfLightbow = table.Column<string>(type: "text", nullable: true),
                    VfHeavybow = table.Column<string>(type: "text", nullable: true),
                    VfCrossbow = table.Column<string>(type: "text", nullable: true),
                    VfOneHandStaff = table.Column<string>(type: "text", nullable: true),
                    VfTwoHandStaff = table.Column<string>(type: "text", nullable: true),
                    VfShieldOnly = table.Column<string>(type: "text", nullable: true),
                    VfIsNotNeedWeapon = table.Column<string>(type: "text", nullable: true),
                    DelayCast = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCastPerSkl = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCastModePerEnhance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCommon = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCooltime = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCooltimePerSkl = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DelayCooltimeModePerEnhance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CoolTimeGroupId = table.Column<int>(type: "integer", nullable: false),
                    UfSelf = table.Column<string>(type: "text", nullable: true),
                    UfParty = table.Column<string>(type: "text", nullable: true),
                    UfGuild = table.Column<string>(type: "text", nullable: true),
                    UfNeutral = table.Column<string>(type: "text", nullable: true),
                    UfPurple = table.Column<string>(type: "text", nullable: true),
                    UfEnemy = table.Column<string>(type: "text", nullable: true),
                    TfAvatar = table.Column<string>(type: "text", nullable: true),
                    TfSummon = table.Column<string>(type: "text", nullable: true),
                    TfMonster = table.Column<string>(type: "text", nullable: true),
                    SkillLvupLimit = table.Column<string>(type: "text", nullable: true),
                    Target = table.Column<int>(type: "integer", nullable: false),
                    EffectType = table.Column<int>(type: "integer", nullable: false),
                    SkillEnchantLinkId = table.Column<int>(type: "integer", nullable: false),
                    StateId = table.Column<int>(type: "integer", nullable: false),
                    StateLevelBase = table.Column<int>(type: "integer", nullable: false),
                    StateLevelPerSkl = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
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
                    Var = table.Column<decimal[]>(type: "numeric[]", maxLength: 19, precision: 10, scale: 2, nullable: true),
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TextId = table.Column<int>(type: "integer", nullable: false),
                    TooltipId = table.Column<int>(type: "integer", nullable: false),
                    IsHarmful = table.Column<bool>(type: "boolean", nullable: false),
                    StateTimeType = table.Column<short>(type: "smallint", nullable: false),
                    StateGroup = table.Column<int>(type: "integer", nullable: false),
                    DuplicateGroup = table.Column<int[]>(type: "integer[]", maxLength: 3, nullable: true),
                    UfAvatar = table.Column<string>(type: "text", nullable: true),
                    UfSummon = table.Column<string>(type: "text", nullable: true),
                    UfMonster = table.Column<string>(type: "text", nullable: true),
                    ReiterationCount = table.Column<string>(type: "text", nullable: true),
                    BaseEffect = table.Column<int>(type: "integer", nullable: false),
                    FireInterval = table.Column<int>(type: "integer", nullable: false),
                    ElementalType = table.Column<int>(type: "integer", nullable: false),
                    AmplifyBase = table.Column<decimal>(type: "numeric(13,3)", precision: 13, scale: 3, nullable: false),
                    AmplifyPerSkill = table.Column<decimal>(type: "numeric(13,3)", precision: 13, scale: 3, nullable: false),
                    AddDamageBase = table.Column<int>(type: "integer", nullable: false),
                    AddDamagePerSkl = table.Column<int>(type: "integer", nullable: false),
                    EffectType = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int[]>(type: "integer[]", maxLength: 20, precision: 13, scale: 3, nullable: true),
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Str = table.Column<int>(type: "integer", nullable: false),
                    Vit = table.Column<int>(type: "integer", nullable: false),
                    Dex = table.Column<int>(type: "integer", nullable: false),
                    Agi = table.Column<int>(type: "integer", nullable: false),
                    Int = table.Column<int>(type: "integer", nullable: false),
                    Men = table.Column<int>(type: "integer", nullable: false),
                    Luk = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StringResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MagicType = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<decimal>(type: "numeric", nullable: false),
                    Scale = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetFxSize = table.Column<decimal>(type: "numeric", nullable: false),
                    StandardWalkSpeed = table.Column<int>(type: "integer", nullable: false),
                    StandardRunSpeed = table.Column<int>(type: "integer", nullable: false),
                    RidingSpeed = table.Column<int>(type: "integer", nullable: false),
                    RunSpeed = table.Column<int>(type: "integer", nullable: false),
                    RidingMotionType = table.Column<int>(type: "integer", nullable: false),
                    AttackRange = table.Column<decimal>(type: "numeric", nullable: false),
                    WalkType = table.Column<int>(type: "integer", nullable: false),
                    SlantType = table.Column<int>(type: "integer", nullable: false),
                    Material = table.Column<int>(type: "integer", nullable: false),
                    WeaponType = table.Column<int>(type: "integer", nullable: false),
                    AttackMotionSpeed = table.Column<int>(type: "integer", nullable: false),
                    EvolutionStage = table.Column<int>(type: "integer", nullable: false),
                    EvolveTargetId = table.Column<int>(type: "integer", nullable: false),
                    CameraX = table.Column<int>(type: "integer", nullable: false),
                    CameraY = table.Column<int>(type: "integer", nullable: false),
                    CameraZ = table.Column<int>(type: "integer", nullable: false),
                    TargetX = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetY = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetZ = table.Column<decimal>(type: "numeric", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: true),
                    MotionFileId = table.Column<int>(type: "integer", nullable: false),
                    FaceId = table.Column<int>(type: "integer", nullable: false),
                    FaceFileName = table.Column<string>(type: "text", nullable: true),
                    CardId = table.Column<int>(type: "integer", nullable: false),
                    ScriptText = table.Column<string>(type: "text", nullable: true),
                    IllustFileName = table.Column<string>(type: "text", nullable: true),
                    TextFeatureId = table.Column<int>(type: "integer", nullable: false),
                    TextNameId = table.Column<int>(type: "integer", nullable: false),
                    Skill1Id = table.Column<int>(type: "integer", nullable: false),
                    Skill1TextId = table.Column<int>(type: "integer", nullable: false),
                    Skill2Id = table.Column<int>(type: "integer", nullable: false),
                    Skill2TextId = table.Column<int>(type: "integer", nullable: false),
                    Skill3Id = table.Column<int>(type: "integer", nullable: false),
                    Skill3TextId = table.Column<int>(type: "integer", nullable: false),
                    Skill4Id = table.Column<int>(type: "integer", nullable: false),
                    Skill4TextId = table.Column<int>(type: "integer", nullable: false),
                    Skill5Id = table.Column<int>(type: "integer", nullable: false),
                    Skill5TextId = table.Column<int>(type: "integer", nullable: false),
                    StatId = table.Column<int>(type: "integer", nullable: false),
                    NameId = table.Column<int>(type: "integer", nullable: false),
                    TextureGroup = table.Column<int>(type: "integer", nullable: false),
                    LocalFlag = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummonResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummonResources_SummonResources_EvolveTargetId",
                        column: x => x.EvolveTargetId,
                        principalTable: "SummonResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SummonResources_EvolveTargetId",
                table: "SummonResources",
                column: "EvolveTargetId");
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
