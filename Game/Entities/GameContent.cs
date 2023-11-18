using System.Collections.Generic;

using Navislamia.Data.Entities;

namespace Navislamia.Entities.Content
{
    public static class GameContent
    {
        public static List<StringResource> Strings { get; set; } = new();

        public static List<MonsterBase> MonsterInfo { get; set; } = new();

        public static List<NPCBase> NpcInfo { get; set; } = new();

    }
}