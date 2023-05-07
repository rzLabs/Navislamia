using System;

namespace Navislamia.Maps.X2D;

public class BoxF
{
    public PointF Begin;
    public PointF End;
    
    public BoxF(PointF begin, PointF end)
    {
        Begin = begin;
        End = end;

        Normalize();
    }

    public BoxF(float left, float top, float right, float bottom)
    {
        Begin = new PointF(left, top);
        End = new PointF(right, bottom);

        Normalize();
    }

    public float GetX() => Begin.X;

    public float GetY() => Begin.Y;

    public float GetLeft() => Begin.X;

    public float GetTop() => Begin.Y;

    public float GetRight() => End.X;

    public float GetBottom() => End.Y;

    public float GetWidth() => End.X - Begin.X + 1;

    public float GetHeight() => End.Y - Begin.Y + 1;

    public void SetLeft(float x)
    {
        Begin.X = x;

        if (Begin.X > End.X)
        {
            (Begin.X, End.X) = (End.X, Begin.X);
        }
    }

    public void SetTop(float y)
    {
        Begin.Y = y;

        if (Begin.Y > End.Y)
        {
            (Begin.Y, End.Y) = (End.Y, Begin.Y);
        }
    }

    public void SetRight(float x)
    {
        End.X = x;

        if (Begin.X > End.X)
        {
            (Begin.X, End.X) = (End.X, Begin.X);
        }
    }

    public void SetBottom(float y)
    {
        End.Y = y;

        if (Begin.Y > End.Y)
        {
            (Begin.Y, End.Y) = (End.Y, Begin.Y);
        }
    }

    public float GetSize() => GetWidth() * GetHeight();

    public void Move(float x, float y)
    {
        var width = GetWidth();
        var height = GetHeight();

        Begin.Set(x, y);
        End.Set(x + width - 1, y + height - 1);
    }

    public bool Has(PointF p)
    {
        if (Begin == p || End == p)
        {
            return true;
        }

        return new PointF(End.X, Begin.Y) == p || new PointF(Begin.X, End.Y) == p;
    }

    public bool Has(LineF line) =>
        GetSegment(0) == line || GetSegment(1) == line || GetSegment(2) == line || GetSegment(3) == line;

    public PointF GetCenter() => new PointF((GetLeft() + GetRight()) / 2, (GetTop() + GetBottom()) / 2);

    public bool IsInclude(float x, float y) => !(Begin.X > x || End.X < x || Begin.Y > y || End.Y < y);

    public bool IsIncluded(PointF point) => IsInclude(point.X, point.Y);

    public bool IsIncluded(BoxF box) => box.Begin.X >= Begin.X && box.End.X <= End.X && box.Begin.Y >= Begin.Y && box.End.Y <= End.Y;

    public bool IsLooseInclude(float x, float y) => !(Begin.X > x || End.X < x || Begin.Y > y || End.Y < y);

    public bool IsLooseInclude(PointF point) => IsLooseInclude(point.X, point.Y);

    public bool IsLooseInclude(BoxF box) => box.Begin.X >= Begin.X && box.End.X <= End.X && box.Begin.Y >= Begin.Y && box.End.Y <= End.Y;

    public bool IsCollision(PointF point) => IsInclude(point.X, point.Y);

    public bool IsCollision(LineF line)
    {
        if (IsIncluded(line.Begin) || IsIncluded(line.End))
        {
            return true;
        }

        return GetSegment(0).IsCollision(line) || GetSegment(1).IsCollision(line) ||
               GetSegment(2).IsCollision(line) || GetSegment(3).IsCollision(line);
    }

    public bool IsCollision(float left, float top, float right, float bottom) =>
        Math.Min(End.X, right) > Math.Max(Begin.X, left) &&
        Math.Min(End.Y, bottom) > Math.Max(Begin.Y, top);

    public bool IsCollision(BoxF box) =>
        Math.Min(End.X, box.End.X) > Math.Max(Begin.X, box.Begin.X) &&
        Math.Min(End.Y, box.End.Y) > Math.Max(Begin.Y, box.Begin.Y);

    public bool IsLooseCollision(PointF point) => IsLooseInclude(point.X, point.Y);

    public bool IsLooseCollision(LineF line)
    {
        return IsLooseInclude(line.Begin) || IsLooseInclude(line.End);
        // TODO:
        /*
        if( GetSegment( 0 ).IsLooseCollision( line ) || GetSegment( 1 ).IsLooseCollision( line ) ||
		    GetSegment( 2 ).IsLooseCollision( line ) || GetSegment( 3 ).IsLooseCollision( line ) ) return true;
        */
    }

    public bool IsLooseCollision(float left, float top, float right, float bottom) =>
        Math.Min(End.X, right) > Math.Max(Begin.X, left) &&
        Math.Min(End.Y, bottom) > Math.Max(Begin.Y, top);

    public bool IsLooseCollision(BoxF box) =>
        Math.Min(End.X, box.End.X) > Math.Max(Begin.X, box.Begin.X) &&
        Math.Min(End.Y, box.End.Y) > Math.Max(Begin.Y, box.Begin.Y);

    public void Set(PointF begin, PointF end)
    {
        Begin = begin;
        End = end;

        Normalize();
    }

    public void Set(float left, float top, float right, float bottom)
    {
        Begin.Set(left, top);
        End.Set(right, bottom);

        Normalize();
    }

    public LineF GetSegment(int index)
    {
        return index switch
        {
            0 => new LineF(Begin.X, Begin.Y, End.X, Begin.Y),
            1 => new LineF(End.X, Begin.Y, End.X, End.Y),
            2 => new LineF(End.X, End.Y, Begin.X, End.Y),
            3 => new LineF(Begin.X, End.Y, Begin.X, Begin.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0-3!")
        };
    }

    public PointF GetPoint(int index)
    {
        return index switch
        {
            0 => new PointF(Begin.X, Begin.Y),
            1 => new PointF(End.X, Begin.Y),
            2 => new PointF(End.X, End.Y),
            3 => new PointF(Begin.X, End.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(index), "Index must be 0-3!")
        };
    }

    public PointF GetLeftTop() => new PointF(Begin.X, Begin.Y);

    public PointF GetRightTop() => new PointF(End.X, Begin.Y);

    public PointF GetLeftBottom() => new PointF(Begin.X, End.Y);

    public PointF GetRightBottom() => new PointF(End.X, End.Y);

    private void Normalize()
    {
        if (Begin.X > End.X)
        {
            (Begin.X, End.X) = (End.X, Begin.X);
        }

        if (Begin.Y > End.Y)
        {
            (Begin.Y, End.Y) = (End.Y, Begin.Y);
        }
    }

}
