using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World
{
    public static class WorldOption
    {
        public const int SpeedUnit = 30;

        public static int RegionSize = 150;

        public static bool UseRegionDebug = false;

        public const uint MaxLayer = 256;

        public const int VisibleRegionRange = 3;

        public const int VisibleRegionBoxWidth = (VisibleRegionRange * 2 + 1);

        public const int IgnorableVisitTime = 10;

        public static int[][] S_Matrix = new int[VisibleRegionBoxWidth][]
        {
            new int[]{ 0,0,1,1,1,0,0 },
            new int[]{ 0,1,1,1,1,1,0 },
            new int[]{ 1,1,1,1,1,1,1 },
            new int[]{ 1,1,1,1,1,1,1 },
            new int[]{ 1,1,1,1,1,1,1 },
            new int[]{ 0,1,1,1,1,1,0 },
            new int[]{ 0,0,1,1,1,0,0 }
        };

        public static uint GetRegionX(float x) => (uint)(x / RegionSize);

        public static uint GetRegionY(float y) => (uint)(y / RegionSize);

        public static int IsVisibleRegion_Fast(uint rx, uint ry, uint _rx, uint _ry) => S_Matrix[VisibleRegionRange + _rx - rx][VisibleRegionRange + _ry - ry];

        public static int IsVisibleRegion(uint rx, uint ry, uint _rx, uint _ry)
        {
            int nx = (int)(VisibleRegionRange + _rx - rx);
            int ny = (int)(VisibleRegionRange + _ry - ry);

            if (nx < 0 || nx >= VisibleRegionRange)
                return 0;

            if (ny < 0 || ny >= VisibleRegionRange)
                return 0;

            return S_Matrix[nx][ny];
        }

        public static int IsVisiblePoint(uint rx, uint ry, float x, float y) => IsVisibleRegion(rx, ry, GetRegionX(x), GetRegionY(y));

        public static void DoEachVisibleRegion(uint rx, uint ry, Func<int> functor)
        {
            throw new NotImplementedException();
        }
    }
}
