using Navislamia.Game.DataAccess.Entities.Enums;

namespace Navislamia.Game.DataAccess.Entities.Arcadia
{
    public class ChannelResourceEntity : Entity
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public ChannelType ChannelType { get; set; }
    }
}