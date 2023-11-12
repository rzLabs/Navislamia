using System.Collections.Generic;
using Navislamia.World.Repositories.Entities;

namespace Navislamia.Game.Entities
{
    public static class GameContent
    {
        public static List<StringResource> Strings { get; set; } = new();

        public static List<MonsterBase> MonsterInfo { get; set; } = new();

        public static List<NPCBase> NpcInfo { get; set; } = new();

    }
}