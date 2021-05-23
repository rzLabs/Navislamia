using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Navislamia.X2D.X2DUtil;

namespace Navislamia.X2D
{
    public class LineF
    {
        public LineF(PointF _begin, PointF _end)
        {
            Begin = _begin;
            End = _end;
        }

        public LineF(float x1, float y1, float x2, float y2)
        {
            Begin = new PointF(x1, y1);
            End = new PointF(x2, y2);
        }

        public float GetLength() => Begin.GetDistance(End);

        public float GetLeftX() => (Begin.X > End.X) ? End.X : Begin.X;

        public float GetRightX() => (Begin.X > End.X) ? Begin.X : End.X;

        public float GetTopY() => (Begin.Y > End.Y) ? End.Y : Begin.Y;

        public float GetBottomY() => (Begin.Y > End.Y) ? Begin.Y : End.Y;

        public bool Has(PointF p) => Begin == p || End == p;

        public bool IsCollision(LineF rh)
        {
            if (GetLeftX() > rh.GetRightX())
                return false;

            if (GetRightX() < rh.GetLeftX())
                return false;

            if (GetTopY() > rh.GetBottomY())
                return false;

            if (GetBottomY() < rh.GetTopY())
                return false;

            return GetIntersectPoint(rh, null);
        }

        public static bool GetIntersectPoint(PointF p1, PointF p2, PointF p3, PointF p4, PointF result)
        {
            float under = (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);

            if (under == 0)
                return false;

            float _t = (p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X);
            float _s = (p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X = p3.X);

            if (_t == 0 && _s == 0)
                return false;

            if (under > 0)
            {
                if (_t < 0 || _s < 0)
                    return false;

                if (_t > under || _s > under)
                    return false;
            }
            else
            {
                if (_t > 0 || _s > 0)
                    return false;

                if (_t < under || _s < under)
                    return false;
            }

            if (result != null)
            {
                result.X = (p1.X + (_t * (p2.X - p1.X)) / under);
                result.Y = (p1.Y + (_t * (p2.Y - p1.Y)) / under);
            }

            return true;
        }

        public static bool GetIntersectPoint(LineF l1, LineF l2, PointF result) => GetIntersectPoint(l1.Begin, l1.End, l2.Begin, l2.End, result);

        public bool GetIntersectPoint(LineF line, PointF result) => GetIntersectPoint(this, line, result);

        public static IntersectResult IntersectCCW(PointF p1, PointF p2, PointF p3, PointF p4)
        {
            float l1_min_x = Math.Min(p1.X, p2.X);
            float l1_max_x = Math.Max(p1.X, p2.X);
            float l1_min_y = Math.Min(p1.Y, p2.Y);
            float l1_max_y = Math.Max(p1.Y, p2.Y);

            float l2_min_x = Math.Min(p3.X, p4.X);
            float l2_max_x = Math.Max(p3.X, p4.X);
            float l2_min_y = Math.Min(p3.Y, p4.Y);
            float l2_max_y = Math.Max(p3.Y, p4.Y);

            if (l1_min_y > l2_max_y || l2_min_y > l2_max_y)
                return IntersectResult.SEPERATE;

            if (l1_min_x > l2_max_x || l2_min_x > l1_max_x)
                return IntersectResult.SEPERATE;

            CCWResult ccw123, ccw124, ccw341, ccw342;
            PointF _p1, _p2, _p3, _p4;

            if (p1.X > p2.X)
            {
                _p1 = p2;
                _p2 = p1;
            }
            else
            {
                _p1 = p1;
                _p2 = p2;
            }

            if (p3.X > p4.X)
            {
                _p3 = p4;
                _p4 = p3;
            }
            else
            {
                _p3 = p3;
                _p4 = p4;
            }

            ccw123 = CheckCloseWise(_p1, _p2, _p3);
            ccw124 = CheckCloseWise(_p1, _p2, _p4);
            ccw341 = CheckCloseWise(_p3, _p4, _p1);
            ccw342 = CheckCloseWise(_p3, _p4, _p2);

            if ((int)ccw123 * (int)ccw123 < 0 && (int)ccw341 * (int)ccw342 < 0)
                return IntersectResult.INTERSECT;

            if (ccw123 == CCWResult.Parallelism && ccw124 == CCWResult.Parallelism)
            {
                if (_p3.X > _p2.X || _p1.X > _p4.X)
                    return IntersectResult.SEPERATE;
                else
                    return IntersectResult.TOUCH;
            }

            if (ccw123 == CCWResult.Parallelism || ccw124 == CCWResult.Parallelism || ccw341 == CCWResult.Parallelism || ccw342 == CCWResult.Parallelism)
            {
                PointF __p1, __p2;

                if (_p1.Y > _p2.Y)
                {
                    __p1 = _p2;
                    __p2 = _p1;
                }
                else
                {
                    __p1 = _p1;
                    __p2 = _p2;
                }

                if (ccw123 == CCWResult.Parallelism)
                    return (_p1.X > _p3.X || _p3.X > _p2.X || (_p3.X == _p1.X && _p1.X == _p2.X && (__p1.Y > _p3.Y || __p2.Y < _p3.Y))) ? IntersectResult.SEPERATE : IntersectResult.TOUCH;

                if (ccw124 == CCWResult.Parallelism)
                    return (_p1.X > _p4.X || _p4.X > _p2.X || (_p4.X == _p1.X && _p1.X == _p2.X && (__p1.Y > _p4.Y || __p2.Y < _p4.Y))) ? IntersectResult.SEPERATE : IntersectResult.TOUCH;

                PointF __p3, __p4;

                if (_p3.Y > _p4.Y)
                {
                    __p3 = p4;
                    __p4 = p3;
                }
                else
                {
                    __p3 = _p3;
                    __p4 = _p4;
                }

                if (ccw341 == CCWResult.Parallelism)
                    return (_p3.X > _p1.X || _p1.X > _p4.X || (_p1.X == _p3.X && _p3.X == _p4.X && (__p3.Y > _p1.Y || __p4.Y < _p1.Y))) ? IntersectResult.SEPERATE : IntersectResult.TOUCH;

                if (ccw342 == CCWResult.Parallelism)
                    return (_p3.X > _p2.X || _p2.X > _p4.X || (_p2.X == _p3.X && _p3.X == _p4.X && (__p3.Y > _p2.Y || __p4.Y < _p2.Y))) ? IntersectResult.SEPERATE : IntersectResult.TOUCH;
            }

            return IntersectResult.SEPERATE;
        }

        public IntersectResult IntersectCCW(LineF lh, LineF rh) => lh.IntersectCCW(rh);

        public IntersectResult IntersectCCW(PointF p3, PointF p4) => IntersectCCW(Begin, End, p3, p4);

        public IntersectResult IntersectCCW(LineF line) => IntersectCCW(Begin, End, line.Begin, line.End);

        public static bool operator ==(LineF lh, LineF rh) => lh.Begin == rh.Begin && lh.End == rh.End || lh.Begin == rh.End && lh.End == rh.Begin;
        public static bool operator !=(LineF lh, LineF rh) => !(lh == rh);

        public PointF Begin;
        public PointF End;
    }

    public enum IntersectResult
    {
        NONE = -99,
        INTERSECT = 1,
        SEPERATE = -1,
        TOUCH = 0
    }
}
