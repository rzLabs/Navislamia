using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.X2D
{
    public class BoxF
    {
        public BoxF(PointF _begin, PointF _end)
        {
            Begin = _begin;
            End = _end;

            normalize();
        }

        public BoxF(float left, float top, float right, float bottom)
        {
            Begin = new PointF(left, top);
            End = new PointF(right, bottom);

            normalize();
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
                float begX = Begin.X;
                Begin.X = End.X;
                End.X = begX;
            }
        }

        public void SetTop(float y)
        {
            Begin.Y = y;

            if (Begin.Y > End.Y)
            {
                float begY = Begin.Y;
                Begin.Y = End.Y;
                End.Y = begY;
            }
        }

        public void SetRight(float x)
        {
            End.X = x;

            if (Begin.X > End.X)
            {
                float begX = Begin.X;
                Begin.X = End.X;
                End.X = begX;
            }
        }

        public void SetBottom(float y)
        {
            End.Y = y;

            if (Begin.Y > End.Y)
            {
                float begY = Begin.Y;
                Begin.Y = End.Y;
                End.Y = begY;
            }
        }

        public float GetSize() => GetWidth() * GetHeight();

        public void Move(float x, float y)
        {
            float width = GetWidth();
            float height = GetHeight();

            Begin.Set(x, y);
            End.Set(x + width - 1, y + height - 1);
        }

        public bool Has(PointF p)
        {
            if (Begin == p || End == p)
                return true;

            if (new PointF(End.X, Begin.Y) == p || new PointF(Begin.X, End.Y) == p)
                return true;

            return false;
        }

        public bool Has(LineF line) =>
            GetSegment(0) == line || GetSegment(1) == line || GetSegment(2) == line || GetSegment(3) == line;

        public PointF GetCenter() => new PointF((GetLeft() + GetRight()) / 2, (GetTop() + GetBottom()) / 2);

        public bool IsInclude(float x, float y) => !(Begin.X > x || End.X < x || Begin.Y > y || End.Y < y);

        public bool IsInclude(PointF pt) => IsInclude(pt.X, pt.Y);

        public bool IsInclude(BoxF bx) => (bx.Begin.X >= Begin.X && bx.End.X <= End.X && bx.Begin.Y >= Begin.Y && bx.End.Y <= End.Y);

        public bool IsLooseInclude(float x, float y) => !(Begin.X > x || End.X < x || Begin.Y > y || End.Y < y);

        public bool IsLooseInclude(PointF pt) => IsLooseInclude(pt.X, pt.Y);

        public bool IsLooseInclude(BoxF bx) => (bx.Begin.X >= Begin.X && bx.End.X <= End.X && bx.Begin.Y >= Begin.Y && bx.End.Y <= End.Y);

        public bool IsCollision(PointF pt) => IsInclude(pt.X, pt.Y);

        public bool IsCollision(LineF line)
        {
            if (IsInclude(line.Begin) || IsInclude(line.End))
                return true;

            if (GetSegment(0).IsCollision(line) || GetSegment(1).IsCollision(line) || GetSegment(2).IsCollision(line) || GetSegment(3).IsCollision(line))
                return true;

            return false;
        }

        public bool IsCollision(float left, float top, float right, float bottom) =>
            (Math.Min(End.X, right) > Math.Max(Begin.X, left)) &&
            (Math.Min(End.Y, bottom) > Math.Max(Begin.Y, top));

        public bool IsCollision(BoxF c) =>
            (Math.Min(End.X, c.End.X) > Math.Max(Begin.X, c.Begin.X)) &&
            (Math.Min(End.Y, c.End.Y) > Math.Max(Begin.Y, c.Begin.Y));

        public bool IsLooseCollision(PointF pt) => IsLooseInclude(pt.X, pt.Y);

        public bool IsLooseCollision(LineF line)
        {
            if (IsLooseInclude(line.Begin) || IsLooseInclude(line.End))
                return true;

            // TODO:

            /*
            if( GetSegment( 0 ).IsLooseCollision( line ) || GetSegment( 1 ).IsLooseCollision( line ) ||
		        GetSegment( 2 ).IsLooseCollision( line ) || GetSegment( 3 ).IsLooseCollision( line ) ) return true;
            */

            return false;
        }

        public bool IsLooseCollision(float left, float top, float right, float bottom) =>
            Math.Min(End.X, right) > Math.Max(Begin.X, left) &&
            Math.Min(End.Y, bottom) > Math.Max(Begin.Y, top);

        public bool IsLooseCollision(BoxF bx) =>
            Math.Min(End.X, bx.End.X) > Math.Max(Begin.X, bx.Begin.X) &&
            Math.Min(End.Y, bx.End.Y) > Math.Max(Begin.Y, bx.Begin.Y);

        public void Set(PointF _begin, PointF _end)
        {
            Begin = _begin;
            End = _end;

            normalize();
        }

        public void Set(float left, float top, float right, float bottom)
        {
            Begin.Set(left, top);
            End.Set(right, bottom);

            normalize();
        }

        public LineF GetSegment(int idx)
        {
            switch (idx)
            {
                case 0:
                    return new LineF(Begin.X, Begin.Y, End.X, Begin.Y);

                case 1:
                    return new LineF(End.X, Begin.Y, End.X, End.Y);

                case 2:
                    return new LineF(End.X, End.Y, Begin.X, End.Y);

                case 3:
                    return new LineF(Begin.X, End.Y, Begin.X, Begin.Y);

                default:
                    throw new ArgumentOutOfRangeException("idx must be 0-3!");
            }
        }

        public PointF GetPoint(int idx)
        {
            switch (idx)
            {
                case 0:
                    return new PointF(Begin.X, Begin.Y);

                case 1:
                    return new PointF(End.X, Begin.Y);

                case 2:
                    return new PointF(End.X, End.Y);

                case 3:
                    return new PointF(Begin.X, End.Y);

                default:
                    throw new ArgumentOutOfRangeException("idx must be 0-3!");
            }
        }

        public PointF GetLeftTop() => new PointF(Begin.X, Begin.Y);

        public PointF GetRightTop() => new PointF(End.X, Begin.Y);

        public PointF GetLeftBottom() => new PointF(Begin.X, End.Y);

        public PointF GetRightBottom() => new PointF(End.X, End.Y);

        void normalize()
        {
            if (Begin.X > End.X)
            {
                float begX = Begin.X;

                Begin.X = End.X;
                End.X = begX;
            }

            if (Begin.Y > End.Y)
            {
                float begY = Begin.Y;

                Begin.Y = End.Y;
                End.Y = begY;
            }
        }

        public PointF Begin;
        public PointF End;
    }
}
