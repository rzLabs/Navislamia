using System;

namespace Navislamia.Game.Maps.X2D;

public class PointF
{
    public PointF(float x, float y)
    {
        X = x;
        Y = y;
    }
    public static PointF operator -(PointF lh, PointF rh) => new PointF(lh.X - rh.X, lh.Y - rh.Y);


    public void Set(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float GetAlternativeDistance(PointF rh)
    {
        var xd = X - rh.X;
        var yd = Y - rh.Y;

        if (xd < 0)
        {
            xd = 0 - xd;
        }

        if (yd < 0)
        {
            yd = 0 - yd;
        }

        return xd + yd;
    }

    public float GetDistance(PointF rh)
    {
        var xd = X - rh.X;
        var yd = Y - rh.Y;

        return (float)Math.Sqrt(xd * xd + yd * yd);
    }

    public float X, Y;
}
