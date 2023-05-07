using Navislamia.Maps.Enums;

namespace Navislamia.Maps.X2D
{
    public static class X2DUtil
    {
        public static CcwResult CheckCloseWise(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            var dx21 = x2 - x1;
            var dy21 = y2 - y1;
            var dx31 = x3 - x1;
            var dy31 = y3 - y1;

            var l = dx21 * dy31 - dy21 * dx31;

            return l switch
            {
                > 0 => CcwResult.ClockWise,
                < 0 => CcwResult.CounterClockWise,
                _ => CcwResult.Parallelism
            };
        }

        public static CcwResult CheckCloseWise(PointF pt1, PointF pt2, PointF pt3) => CheckCloseWise(pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y);

        public static bool Included(RectF lh, PointF rh) => lh.IsIncluded(rh);

        public static bool Included(RectF lh, RectF rh) => lh.IsInclude(rh);

        public static bool Included(RectF lh, BoxF rh) => lh.IsInclude(rh);

        public static bool Included(RectF lh, PolygonF rh) => rh.IsIn(lh);

        public static bool LOOSE_INCLUDE(RectF lh, PointF rh) => lh.IsLooseIncluded(rh);

        public static bool Collision(PolygonF lh, RectF rh) => lh.IsCollision(rh);

        public static bool Collision(RectF lh, PolygonF rh) => Collision(rh, lh);

        public static bool Collision(RectF lh, PointF rh) => lh.IsIncluded(rh);

        public static bool Collision(RectF lh, LineF rh) => lh.IsCollision(rh);

        public static bool Collision(PolygonF lh, PointF rh) => lh.IsCollision(rh);

        public static bool LOOSE_COLLISION(PolygonF lh, LineF rh) => lh.IsLooseCollision(rh);
    }


}
