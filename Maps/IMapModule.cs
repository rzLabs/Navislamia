using Objects;

namespace Navislamia.Maps
{
    public interface IMapModule
    {
        public bool Initialize(string directory);

        public KSize MapCount { get; set; }
    }
}
