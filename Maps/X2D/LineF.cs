using System;
using Navislamia.Maps.Enums;
using static Navislamia.Maps.X2D.X2DUtil;

namespace Navislamia.Maps.X2D;

public class LineF
{
    public readonly PointF Begin;
    public readonly PointF End;
    
    public LineF(PointF begin, PointF end)
    {
        Begin = begin;
        End = end;
    }

    public LineF(float xBegin, float yBegin, float xEnd, float yEnd)
    {
        Begin = new PointF(xBegin, yBegin);
        End = new PointF(xEnd, yEnd);
    }

    private bool Equals(LineF other)
    {
        return Equals(Begin, other.Begin) && Equals(End, other.End);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }
        
        return Equals((LineF)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Begin, End);
    }

    public float GetLength() => Begin.GetDistance(End);

    public float GetLeftX() => Begin.X > End.X ? End.X : Begin.X;

    public float GetRightX() => Begin.X > End.X ? Begin.X : End.X;

    public float GetTopY() => Begin.Y > End.Y ? End.Y : Begin.Y;

    public float GetBottomY() => Begin.Y > End.Y ? Begin.Y : End.Y;

    public bool Has(PointF p) => Begin == p || End == p;

    public bool IsCollision(LineF rh)
    {
        if (GetLeftX() > rh.GetRightX())
        {
            return false;
        }

        if (GetRightX() < rh.GetLeftX())
        {
            
            return false;
        }

        if (GetTopY() > rh.GetBottomY())
        {
            return false;
        }

        if (GetBottomY() < rh.GetTopY())
        {
            return false;
        }

        return GetIntersectPoint(rh, null);
    }

    public static bool GetIntersectPoint(PointF point1, PointF point2, PointF point3, PointF point4, PointF result)
    {
        float under = (point4.Y - point3.Y) * (point2.X - point1.X) - (point4.X - point3.X) * (point2.Y - point1.Y);

        if (under == 0)
        {
            return false;
        }

        float t = (point4.X - point3.X) * (point1.Y - point3.Y) - (point4.Y - point3.Y) * (point1.X - point3.X);
        float s = (point2.X - point1.X) * (point1.Y - point3.Y) - (point2.Y - point1.Y) * (point1.X = point3.X);

        if (t == 0 && s == 0)
        {
            return false;
        }

        if (under > 0)
        {
            if (t < 0 || s < 0)
            {
                return false;
            }

            if (t > under || s > under)
            {
                return false;
            }
        }
        else
        {
            if (t > 0 || s > 0)
            {
                return false;
            }

            if (t < under || s < under)
            {
                return false;
            }
        }

        if (result == null)
        {
            return true;
        }
        
        result.X = point1.X + t * (point2.X - point1.X) / under;
        result.Y = point1.Y + t * (point2.Y - point1.Y) / under;

        return true;
    }

    public static bool GetIntersectPoint(LineF l1, LineF l2, PointF result) =>
        GetIntersectPoint(l1.Begin, l1.End, l2.Begin, l2.End, result);

    public bool GetIntersectPoint(LineF line, PointF result) => GetIntersectPoint(this, line, result);

    public static IntersectResult IntersectCcw(PointF p1, PointF p2, PointF p3, PointF p4)
    {
        float l1MinX = Math.Min(p1.X, p2.X);
        float l1MaxX = Math.Max(p1.X, p2.X);
        float l1MinY = Math.Min(p1.Y, p2.Y);
        float l1MaxY = Math.Max(p1.Y, p2.Y); // TODO: unused ?

        float l2MinX = Math.Min(p3.X, p4.X);
        float l2MaxX = Math.Max(p3.X, p4.X);
        float l2MinY = Math.Min(p3.Y, p4.Y);
        float l2MaxY = Math.Max(p3.Y, p4.Y);

        if (l1MinY > l2MaxY || l2MinY > l2MaxY)
        {
            return IntersectResult.SEPERATE;
        }

        if (l1MinX > l2MaxX || l2MinX > l1MaxX)
        {
            return IntersectResult.SEPERATE;
        }

        PointF point1;
        PointF point2;
        PointF point3;
        PointF point4;

        if (p1.X > p2.X)
        {
            point1 = p2;
            point2 = p1;
        }
        else
        {
            point1 = p1;
            point2 = p2;
        }

        if (p3.X > p4.X)
        {
            point3 = p4;
            point4 = p3;
        }
        else
        {
            point3 = p3;
            point4 = p4;
        }

        var ccw123 = CheckCloseWise(point1, point2, point3);
        var ccw124 = CheckCloseWise(point1, point2, point4);
        var ccw341 = CheckCloseWise(point3, point4, point1);
        var ccw342 = CheckCloseWise(point3, point4, point2);

        if ((int)ccw123 * (int)ccw123 < 0 && (int)ccw341 * (int)ccw342 < 0)
        {
            return IntersectResult.INTERSECT;
        }

        if (ccw123 == CcwResult.Parallelism && ccw124 == CcwResult.Parallelism)
        {
            if (point3.X > point2.X || point1.X > point4.X)
            {
                return IntersectResult.SEPERATE;
            }
            return IntersectResult.TOUCH;
        }

        if (ccw123 != CcwResult.Parallelism && ccw124 != CcwResult.Parallelism && 
            ccw341 != CcwResult.Parallelism && ccw342 != CcwResult.Parallelism)
        {
            return IntersectResult.SEPERATE;
        }
        
        PointF tmpPoint1;
        PointF tmpPoint2;

        if (point1.Y > point2.Y)
        {
            tmpPoint1 = point2;
            tmpPoint2 = point1;
        }
        else
        {
            tmpPoint1 = point1;
            tmpPoint2 = point2;
        }

        if (ccw123 == CcwResult.Parallelism)
        {
            return point1.X > point3.X ||
                   point3.X > point2.X ||
                   (point3.X == point1.X && point1.X == point2.X && (tmpPoint1.Y > point3.Y || tmpPoint2.Y < point3.Y))
                ? IntersectResult.SEPERATE
                : IntersectResult.TOUCH;
        }

        if (ccw124 == CcwResult.Parallelism)
        {
            return point1.X > point4.X ||
                   point4.X > point2.X ||
                   (point4.X == point1.X && point1.X == point2.X && (tmpPoint1.Y > point4.Y || tmpPoint2.Y < point4.Y))
                ? IntersectResult.SEPERATE
                : IntersectResult.TOUCH;
        }

        PointF tmpPoint3;
        PointF tmpPoint4;

        if (point3.Y > point4.Y)
        {
            tmpPoint3 = p4;
            tmpPoint4 = p3;
        }
        else
        {
            tmpPoint3 = point3;
            tmpPoint4 = point4;
        }

        if (ccw341 == CcwResult.Parallelism)
        {
            return point3.X > point1.X ||
                   point1.X > point4.X ||
                   (point1.X == point3.X && point3.X == point4.X && (tmpPoint3.Y > point1.Y || tmpPoint4.Y < point1.Y))
                ? IntersectResult.SEPERATE
                : IntersectResult.TOUCH;
        }

        if (ccw342 == CcwResult.Parallelism)
        {
            return point3.X > point2.X ||
                   point2.X > point4.X ||
                   (point2.X == point3.X && point3.X == point4.X && (tmpPoint3.Y > point2.Y || tmpPoint4.Y < point2.Y))
                ? IntersectResult.SEPERATE
                : IntersectResult.TOUCH;
        }

        return IntersectResult.SEPERATE;
    }

    public IntersectResult IntersectCcw(LineF lh, LineF rh) => lh.IntersectCcw(rh);

    public IntersectResult IntersectCcw(PointF p3, PointF p4) => IntersectCcw(Begin, End, p3, p4);

    public IntersectResult IntersectCcw(LineF line) => IntersectCcw(Begin, End, line.Begin, line.End);

    public static bool operator ==(LineF lh, LineF rh) => 
        lh?.Begin == rh?.Begin && lh?.End == rh?.End || lh?.Begin == rh?.End && lh?.End == rh?.Begin;
    
    public static bool operator !=(LineF lh, LineF rh) => !(lh == rh);
}
