using System;
using Navislamia.Game.Maps.Enums;

namespace Navislamia.Game.Maps.X2D;
public class RectF
{
    private PointF _pos;
    private PointF _size;
    
    public RectF(PointF pos, PointF size)
    {
        _pos = pos;
        _size = size;
    }

    public RectF(float x, float y, float width, float height)
    {
        _pos = new PointF(x, y);
        _size = new PointF(width, height);
    }

    public float GetX() => _pos.X;

    public float GetY() => _pos.Y;

    public float GetWidth() => _size.X;

    public float GetHeight() => _size.Y;

    public float GetSize() => GetWidth() * GetHeight();

    public void Move(float x, float y) => _pos.Set(x, y);

    public TouchPosition GetTouchPosition(RectF rc)
    {
        if (IsCollision(rc))
        {
            return TouchPosition.COLLISION;
        }

        var bX = (rc.GetX() >= GetX() && GetX() < GetRight()) ||
                 (GetX() >= rc.GetX() && GetX() < rc.GetRight());

        var bY = (rc.GetY() >= GetY() && rc.GetY() < GetBottom()) ||
            (GetY() >= rc.GetY() && GetY() < rc.GetBottom());

        if (bX)
        {
            if (rc.GetY() + rc.GetHeight() == GetY())
            {
                return TouchPosition.TOP;
            }

            if (GetY() + GetHeight() == rc.GetY())
            {
                return TouchPosition.BOTTOM;
            }
        }

        if (!bY)
        {
            return TouchPosition.SEPERATE;
        }
        
        if (rc.GetX() + rc.GetWidth() == GetX())
        {
            return TouchPosition.LEFT;
        }

        if (GetX() + GetWidth() == rc.GetX())
        {
            return TouchPosition.RIGHT;
        }

        return TouchPosition.SEPERATE;
    }

    public bool IsInclude(float x, float y) => GetX() <= x && GetRight() > x && GetY() <= y && GetBottom() > y;

    public bool IsIncluded(PointF pt) => IsInclude(pt.X, pt.Y);

    public bool IsInclude(RectF c) => GetX() <= c.GetX() && GetRight() >= c.GetRight() && GetY() <= c.GetY() && GetBottom() >= c.GetBottom();

    public bool IsInclude(BoxF c) => GetX() <= c.GetLeft() && GetRight() > c.GetRight() && GetTop() <= c.GetY() && GetBottom() > c.GetBottom();

    public bool IsLooseInclude(float x, float y) => GetX() < x && GetRight() > x && GetY() < y && GetBottom() > y;

    public bool IsLooseIncluded(PointF pt) => IsLooseInclude(pt.X, pt.Y);

    public bool IsLooseInclude(RectF c) => GetX() < c.GetX() && GetRight() > c.GetRight() && GetY() < c.GetY() && GetBottom() > c.GetBottom();

    public bool IsLooseInclude(BoxF c) => GetX() < c.GetLeft() && GetRight() > c.GetRight() && GetTop() < c.GetY() && GetBottom() > c.GetBottom();

    public bool IsCollision(PointF pt) => IsInclude(pt.X, pt.Y);

    public bool IsCollision(LineF line)
    {
        if (_size.X == 0 || _size.Y == 0)
        {
            return false;
        }

        PointF middle = new ((line.Begin.X + line.End.X) / 2, (line.Begin.Y + line.End.Y) / 2);

        if (IsIncluded(middle) || IsIncluded(line.Begin) || IsIncluded(line.End))
        {
            return true;
        }

        LineF top = new (_pos.X, _pos.Y, _pos.X + _size.X, _pos.Y);
        LineF bottom = new (_pos.X, _pos.Y + _size.Y, _pos.X + _size.X, _pos.Y + _size.Y);
        LineF left = new (_pos.X, _pos.Y, _pos.X, _pos.Y + _size.Y);
        LineF right = new (_pos.X + _size.X, _pos.Y, _pos.X + _size.X, _pos.Y + _size.Y);

        var result1 = line.IntersectCcw(top);

        if (result1 == IntersectResult.INTERSECT)
        {
            return true;
        }

        var result2 = line.IntersectCcw(bottom);

        if (result2 == IntersectResult.INTERSECT)
        {
            return true;
        }

        var result3 = line.IntersectCcw(left);

        if (result3 == IntersectResult.INTERSECT)
        {
            return true;
        }

        var result4 = line.IntersectCcw(right);

        if (result4 == IntersectResult.INTERSECT)
        {
            return true;
        }

        if (result3 == IntersectResult.TOUCH)
        {
            return result2 != IntersectResult.TOUCH;
        }

        if (result1 == IntersectResult.TOUCH)
        {
            return result4 != IntersectResult.TOUCH;
        }
        
        return false;
    }

    public bool IsCollision(RectF c) =>
        Math.Min(GetRight(), c.GetRight()) > Math.Max(GetX(), c.GetX()) &&
        Math.Min(GetBottom(), c.GetBottom()) > Math.Max(GetY(), c.GetY());

    public bool IsCollision(BoxF bx)
    {
        var bX = (GetX() >= bx.GetLeft() && GetX() <= bx.GetRight()) ||
            (bx.GetLeft() >= GetX() && bx.GetLeft() < GetRight());

        var bY = (GetY() >= bx.GetTop() && GetY() <= bx.GetBottom()) ||
            (bx.GetTop() >= GetTop() && bx.GetTop() < GetBottom());

        return bX && bY;
    }

    public bool IsLooseCollision(PointF pt) => IsLooseInclude(pt.X, pt.Y);

    public bool IsLooseCollision(LineF line)
    {
        if (_size.X == 0 || _size.Y == 0)
        {
            return false;
        }

        PointF middle = new ((line.Begin.X + line.End.X) / 2, (line.Begin.Y + line.End.Y) / 2);

        if (IsLooseIncluded(middle) || IsLooseIncluded(line.Begin) || IsLooseIncluded(line.End))
        {
            return true;
        }

        LineF top = new (_pos.X, _pos.Y, _pos.X + _size.X, _pos.Y);
        LineF bottom = new (_pos.X, _pos.Y + _size.Y, _pos.X + _size.X, _pos.Y + _size.Y);
        LineF left = new (_pos.X, _pos.Y, _pos.X, _pos.Y + _size.Y);
        LineF right = new (_pos.X + _size.X, _pos.Y, _pos.X + _size.X, _pos.Y + _size.Y);

        var result1 = line.IntersectCcw(top);

        if (result1! == IntersectResult.INTERSECT)
            return true;

        var result2 = line.IntersectCcw(bottom);

        if (result2 == IntersectResult.INTERSECT)
            return true;

        var result3 = line.IntersectCcw(left);

        if (result3 == IntersectResult.INTERSECT)
            return true;

        var result4 = line.IntersectCcw(right);

        if (result4 == IntersectResult.INTERSECT)
        {
            return true;
        }

        return false;
    }

    public bool IsLooseCollision(RectF r) =>
        Math.Min(GetRight(), r.GetRight()) > Math.Max(GetX(), r.GetX()) &&
        Math.Min(GetBottom(), r.GetBottom()) > Math.Max(GetY(), r.GetY());

    public bool IsLooseCollision(BoxF bx)
    {
        var bX = (GetX() > bx.GetLeft() && GetX() < bx.GetRight()) ||
                 (bx.GetLeft() > GetX() && bx.GetLeft() < GetRight());

        var bY = (GetY() >= bx.GetTop() && GetY() <= bx.GetBottom()) ||
                 (bx.GetTop() >= GetY() && bx.GetTop() < GetBottom());

        return bX && bY;
    }

    public void Set(PointF pos, PointF size)
    {
        _pos = pos;
        _size = size;
    }

    public void SetWidth(float width) => _size.X = width;

    public void SetHeight(float height) => _size.Y = height;

    public void Set(float x, float y, float width, float height)
    {
        _pos.Set(x, y);
        _size.Set(width, height);
    }

    public float GetLeft() => _pos.X;

    public float GetTop() => _pos.Y;

    public float GetRight() => _pos.X + _size.X;

    public float GetBottom() => _pos.Y + _size.Y;

    public PointF GetLeftTop() => new PointF(GetLeft(), GetTop());

    public PointF GetRightTop() => new PointF(GetRight(), GetTop());

    public PointF GetLeftBottom() => new PointF(GetLeft(), GetBottom());

    public PointF GetRightBottom() => new PointF(GetRight(), GetBottom());
}
