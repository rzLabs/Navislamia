using Objects;

namespace Maps
{
    public interface IMapService
    {
        public bool Initialize(string directory);

        public KSize MapCount { get; set; }
    }
}
