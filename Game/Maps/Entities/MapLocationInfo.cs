using Navislamia.Maps.X2D;

namespace Navislamia.Maps.Entities;

public class MapLocationInfo : PolygonF
{
    public MapLocationInfo() { }

    public MapLocationInfo(PolygonF p) : base(p)
    {
        LocationId = 0;
        Priority = 0;
    }

    public int LocationId;
    public int Priority;
}
