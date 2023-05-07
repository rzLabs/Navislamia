using System.Collections.Generic;
using Navislamia.Database.Entities;

namespace Navislamia.Game.Entities
{
    public static class GameContent
    {
        public static List<StringResource> Strings { get; set; } = new();

        public static List<MonsterBase> MonsterInfo { get; set; } = new();

        public static List<NPCBase> NpcInfo { get; set; } = new();

    }
}