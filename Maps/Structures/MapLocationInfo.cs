using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Objects.X2D;

namespace Maps.Structures
{
    public class MapLocationInfo : PolygonF
    {
        public MapLocationInfo() { }

        public MapLocationInfo(PolygonF p) : base(p)
        {
            LocationID = 0;
            Priority = 0;
        }

        public int LocationID;
        public int Priority;
    }
}
