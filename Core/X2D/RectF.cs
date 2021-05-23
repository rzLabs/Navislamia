using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.X2D
{
    public class RectF
    {
        public RectF(PointF _pos, PointF _size)
        {
            pos = _pos;
            size = _size;
        }

        public RectF(float x, float y, float width, float height)
        {
            pos = new PointF(x, y);
            size = new PointF(width, height);
        }

        public float GetX() => pos.X;

        public float GetY() => pos.Y;

        public float GetWidth() => size.X;

        public float GetHeight() => size.Y;

        public float GetSize() => GetWidth() * GetHeight();

        public void Move(float x, float y) => pos.Set(x, y);

        public TouchPosition GetTouchPosition(RectF rc)
        {
            if (IsCollision(rc))
                return TouchPosition.COLLISION;

            bool bX = (rc.GetX() >= GetX() && GetX() < GetRight()) ||
                (GetX() >= rc.GetX() && GetX() < rc.GetRight());

            bool bY = (rc.GetY() >= GetY() && rc.GetY() < GetBottom()) ||
                (GetY() >= rc.GetY() && GetY() < rc.GetBottom());

            if (bX)
            {
                if (rc.GetY() + rc.GetHeight() == GetY())
                    return TouchPosition.TOP;

                if (GetY() + GetHeight() == rc.GetY())
                    return TouchPosition.BOTTOM;
            }

            if (bY)
            {
                if (rc.GetX() + rc.GetWidth() == GetX())
                    return TouchPosition.LEFT;

                if (GetX() + GetWidth() == rc.GetX())
                    return TouchPosition.RIGHT;
            }

            return TouchPosition.SEPERATE;
        }

        public bool IsInclude(float x, float y) => (GetX() <= x && GetRight() > x && GetY() <= y && GetBottom() > y);

        public bool IsInclude(PointF pt) => IsInclude(pt.X, pt.Y);

        public bool IsInclude(RectF c) => (GetX() <= c.GetX() && GetRight() >= c.GetRight() && GetY() <= c.GetY() && GetBottom() >= c.GetBottom());

        public bool IsInclude(BoxF c) => (GetX() <= c.GetLeft() && GetRight() > c.GetRight() && GetTop() <= c.GetY() && GetBottom() > c.GetBottom());

        public bool IsLooseInclude(float x, float y) => (GetX() < x && GetRight() > x && GetY() < y && GetBottom() > y);

        public bool IsLooseInclude(PointF pt) => IsLooseInclude(pt.X, pt.Y);

        public bool IsLooseInclude(RectF c) => (GetX() < c.GetX() && GetRight() > c.GetRight() && GetY() < c.GetY() && GetBottom() > c.GetBottom());

        public bool IsLooseInclude(BoxF c) => (GetX() < c.GetLeft() && GetRight() > c.GetRight() && GetTop() < c.GetY() && GetBottom() > c.GetBottom());

        public bool IsCollision(PointF pt) => IsInclude(pt.X, pt.Y);

        public bool IsCollision(LineF line)
        {
            if (size.X == 0 || size.Y == 0)
                return false;

            PointF middle = new PointF((line.Begin.X + line.End.X) / 2, (line.Begin.Y + line.End.Y) / 2);

            if (IsInclude(middle) || IsInclude(line.Begin) || IsInclude(line.End))
                return true;

            LineF top = new LineF(pos.X, pos.Y, pos.X + size.X, pos.Y);
            LineF bottom = new LineF(pos.X, pos.Y + size.Y, pos.X + size.X, pos.Y + size.Y);
            LineF left = new LineF(pos.X, pos.Y, pos.X, pos.Y + size.Y);
            LineF right = new LineF(pos.X + size.X, pos.Y, pos.X + size.X, pos.Y + size.Y);

            IntersectResult result1 = line.IntersectCCW(top);

            if (result1 == IntersectResult.INTERSECT)
                return true;

            IntersectResult result2 = line.IntersectCCW(bottom);

            if (result2 == IntersectResult.INTERSECT)
                return true;

            IntersectResult result3 = line.IntersectCCW(left);

            if (result3 == IntersectResult.INTERSECT)
                return true;

            IntersectResult result4 = line.IntersectCCW(right);

            if (result4 == IntersectResult.INTERSECT)
                return true;

            if (result3 == IntersectResult.TOUCH)
                return (result2 != IntersectResult.TOUCH);

            if (result1 == IntersectResult.TOUCH)
                return result4 != IntersectResult.TOUCH;


            return false;
        }

        public bool IsCollision(RectF c) =>
            Math.Min(GetRight(), c.GetRight()) > Math.Max(GetX(), c.GetX()) &&
            Math.Min(GetBottom(), c.GetBottom()) > Math.Max(GetY(), c.GetY());

        public bool IsCollision(BoxF bx)
        {
            bool bX = (GetX() >= bx.GetLeft() && GetX() <= bx.GetRight()) ||
                (bx.GetLeft() >= GetX() && bx.GetLeft() < GetRight());

            bool bY = (GetY() >= bx.GetTop() && GetY() <= bx.GetBottom()) ||
                (bx.GetTop() >= GetTop() && bx.GetTop() < GetBottom());

            return bX && bY;
        }

        public bool IsLooseCollision(PointF pt) => IsLooseInclude(pt.X, pt.Y);

        public bool IsLooseCollision(LineF line)
        {
            if (size.X == 0 || size.Y == 0)
                return false;

            PointF middle = new PointF((line.Begin.X + line.End.X) / 2, (line.Begin.Y + line.End.Y) / 2);

            if (IsLooseInclude(middle) || IsLooseInclude(line.Begin) || IsLooseInclude(line.End))
                return true;

            LineF top = new LineF(pos.X, pos.Y, pos.X + size.X, pos.Y);
            LineF bottom = new LineF(pos.X, pos.Y + size.Y, pos.X + size.X, pos.Y + size.Y);
            LineF left = new LineF(pos.X, pos.Y, pos.X, pos.Y + size.Y);
            LineF right = new LineF(pos.X + size.X, pos.Y, pos.X + size.X, pos.Y + size.Y);

            IntersectResult result1 = line.IntersectCCW(top);

            if (result1! == IntersectResult.INTERSECT)
                return true;

            IntersectResult result2 = line.IntersectCCW(bottom);

            if (result2 == IntersectResult.INTERSECT)
                return true;

            IntersectResult result3 = line.IntersectCCW(left);

            if (result3 == IntersectResult.INTERSECT)
                return true;

            IntersectResult result4 = line.IntersectCCW(right);

            if (result4 == IntersectResult.INTERSECT)
                return true;

            return false;
        }

        public bool IsLooseCollision(RectF r) =>
            (Math.Min(GetRight(), r.GetRight()) > Math.Max(GetX(), r.GetX())) &&
            (Math.Min(GetBottom(), r.GetBottom()) > Math.Max(GetY(), r.GetY()));

        public bool IsLooseCollision(BoxF bx)
        {
            bool bX = (GetX() > bx.GetLeft() && GetX() < bx.GetRight()) ||
                (bx.GetLeft() > GetX() && bx.GetLeft() < GetRight());

            bool bY = (GetY() >= bx.GetTop() && GetY() <= bx.GetBottom()) ||
                (bx.GetTop() >= GetY() && bx.GetTop() < GetBottom());

            return bX && bY;
        }

        public void Set(PointF _pos, PointF _size)
        {
            pos = _pos;
            size = _size;
        }

        public void SetWidth(float width) => size.X = width;

        public void SetHeight(float height) => size.Y = height;

        public void Set(float x, float y, float width, float height)
        {
            pos.Set(x, y);
            size.Set(width, height);
        }

        public float GetLeft() => pos.X;

        public float GetTop() => pos.Y;

        public float GetRight() => pos.X + size.X;

        public float GetBottom() => pos.Y + size.Y;

        public PointF GetLeftTop() => new PointF(GetLeft(), GetTop());

        public PointF GetRightTop() => new PointF(GetRight(), GetTop());

        public PointF GetLeftBottom() => new PointF(GetLeft(), GetBottom());

        public PointF GetRightBottom() => new PointF(GetRight(), GetBottom());

        PointF pos;
        PointF size;
    }

    public enum TouchPosition
    {
        SEPERATE = -1,
        LEFT = 0,
        TOP = 1,
        RIGHT = 2,
        BOTTOM = 3,
        COLLISION = 4
    }
}
