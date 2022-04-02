using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.X2D
{
    public class PointF
    {
        public PointF(float _x, float _y)
        {
            X = _x;
            Y = _y;
        }
        public static PointF operator -(PointF lh, PointF rh) => new PointF(lh.X - rh.X, lh.Y - rh.Y);


        public void Set(float _x, float _y)
        {
            X = _x;
            Y = _y;
        }

        public float GetAlternativeDistance(PointF rh)
        {
            float xd = X - rh.X;
            float yd = Y - rh.Y;

            if (xd < 0)
                xd = 0 - xd;

            if (yd < 0)
                yd = 0 - yd;

            return xd + yd;
        }

        public float GetDistance(PointF rh)
        {
            float xd = X - rh.X;
            float yd = Y - rh.Y;

            return (float)Math.Sqrt(xd * xd + yd * yd);
        }

        public float X, Y;
    }

    public struct Point3D
    {
        public float X, Y, Z;
    }
}
