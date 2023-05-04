using System;
using Navislamia.Network.Enums;

namespace Navislamia.Network.Extensions;

public static class EnumExtensions
{
    public static string EnumToString(this Enum input) => Enum.GetName(typeof(ClientType), input);
}