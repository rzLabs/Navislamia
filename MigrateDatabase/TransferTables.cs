namespace MigrateDatabase;

[Flags]
public enum TransferTables : short
{
    Unset = 0,
    None = 1,
    All = 2,
    ItemResource = 4,
    LevelResource = 8,
    StringResource = 16,
    StatResource = 32,
    ChannelResource = 64,
    GlobaleVariables = 128,
    EffectResource = 256,
    ItemEffectResource = 512,
    SummonResource = 1024,
    SetItemEffectResource = 2048,
    EnhanceResource = 4096,
    SkillResource = 8192,
    StateResource = 16384,
}