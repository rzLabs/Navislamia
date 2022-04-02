using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Objects;

namespace Maps.X2D
{
    public static class X2DUtil
    {
        public static CCWResult CheckCloseWise(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float l;
            float dx21 = x2 - x1;
            float dy21 = y2 - y1;
            float dx31 = x3 - x1;
            float dy31 = y3 - y1;

            l = dx21 * dy31 - dy21 * dx31;

            if (l > 0)
                return CCWResult.ClockWise;
            else if (l < 0)
                return CCWResult.CounterClockWise;
            else
                return CCWResult.Parallelism;
        }

        public static CCWResult CheckCloseWise(PointF pt1, PointF pt2, PointF pt3) => CheckCloseWise(pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y);

        public static bool INCLUDE(RectF lh, PointF rh) => lh.IsInclude(rh);

        public static bool INCLUDE(RectF lh, RectF rh) => lh.IsInclude(rh);

        public static bool INCLUDE(RectF lh, BoxF rh) => lh.IsInclude(rh);

        public static bool INCLUDE(RectF lh, PolygonF rh) => rh.IsIn(lh);

        public static bool LOOSE_INCLUDE(RectF lh, PointF rh) => lh.IsLooseInclude(rh);

        public static bool COLLISION(PolygonF lh, RectF rh) => lh.IsCollision(rh);

        public static bool COLLISION(RectF lh, PolygonF rh) => COLLISION(rh, lh);

        public static bool COLLISION(RectF lh, PointF rh) => lh.IsInclude(rh);

        public static bool COLLISION(RectF lh, LineF rh) => lh.IsCollision(rh);

        public static bool COLLISION(PolygonF lh, PointF rh) => lh.IsCollision(rh);

        public static bool LOOSE_COLLISION(PolygonF lh, LineF rh) => lh.IsLooseCollision(rh);
    }

    public enum CCWResult
    {
        ClockWise = 1,
        CounterClockWise = -1,
        Parallelism = 0
    }
}
