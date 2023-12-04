using System;

namespace DevConsole.Models.Arcadia.Enums;

[Flags]
public enum ItemJobRestriction : short
{
    LimitFighter = 1024,
    LimitHunter = 2048,
    LimitMagician = 4096,
    LimitSummoner = 8192
}