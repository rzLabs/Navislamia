using System;

namespace Navislamia.Game.Models.Enums;

/// <summary>
/// Flags are only used in Arcadia. Telecaster uses bit values
/// </summary>
[Flags]
public enum JobDepth : short
{
    Base = 1,
    First = 2, 
    Second = 4,
    Master = 8,
}