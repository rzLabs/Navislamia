using System;
using System.Collections.Generic;

using Navislamia.Database.Entities;

namespace Navislamia.Game
{
    public static class GameContent
    {
        public static List<StringResource> Strings { get; set; } = new List<StringResource>();

        public static List<MonsterBase> MonsterInfo { get; set; } = new List<MonsterBase>();

        public static List<NPCBase> NpcInfo { get; set; } = new List<NPCBase>();

    }
}