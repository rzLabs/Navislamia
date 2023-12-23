using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Navislamia.Game.Filer
{
    public struct KPoint
    {
        public KPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(KPoint p1, KPoint p2) => p1.X == p2.X && p1.Y == p2.Y;

        public static bool operator !=(KPoint p1, KPoint p2) => p1.X != p2.X || p1.Y != p2.Y;

        public static KPoint operator +(KPoint p1, KPoint p2) => new KPoint(p1.X + p2.X, p1.Y + p2.Y);

        public static KPoint operator -(KPoint p1, KPoint p2) => new KPoint(p1.X - p2.X, p1.Y - p2.Y);

        public int X, Y;
    }

    public struct KSize
    {
        public KSize(int cx, int cy)
        {
            Width = 0;
            Height = 0;
            CX = cx;
            CY = cy;
        }

        public int Width;

        public int CX;

        public int Height;

        public int CY;
    }

    public struct KRect
    {
        public KRect( int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public KRect(KPoint point, KSize size)
        {
            Left = point.X;
            Top = point.Y;
            Right = point.X + size.CX;
            Bottom = point.Y + size.CY;
        }

        public static KRect operator +(KRect k1, KRect k2) => new KRect(k1.Left + k2.Left, k1.Top + k2.Top, k1.Right + k2.Right, k1.Bottom + k2.Bottom);

        public static KRect operator -(KRect k1, KRect k2) => new KRect(k1.Left - k2.Left, k1.Top - k2.Top, k1.Right - k2.Right, k1.Bottom - k2.Bottom);

        public static bool operator ==(KRect k1, KRect k2) => k1.Left == k2.Left && k1.Top == k2.Top && k1.Right == k2.Right && k1.Bottom == k2.Bottom;

        public static bool operator !=(KRect k1, KRect k2) => k1.Left != k2.Left || k1.Top != k2.Top || k1.Right != k2.Right || k1.Bottom != k2.Bottom;

        public bool IsInRect(int x, int y)
        {
            if (Left <= x && Right > x)
                if (Top <= y && Bottom > y)
                    return true;

            return false;
        }

        public int GetWidth() => Right - Left;

        public int GetHeight => Bottom - Top;

        public void Intersect(KRect rect)
        {
            Left = Math.Max(Left, rect.Left);
            Top = Math.Max(Top, rect.Top);
            Right = Math.Min(Right, rect.Right);
            Bottom = Math.Min(Bottom, rect.Bottom);
        }

        public void Union(KRect rect)
        {
            Left = Math.Min(Left, rect.Left);
            Top = Math.Min(Top, rect.Top);
            Right = Math.Max(Right, rect.Right);
            Bottom = Math.Max(Bottom, rect.Bottom);
        }

        public int Left, Top, Right, Bottom;
    }

    public struct KTripleColor
    {
        public KTripleColor(KColor rColor)
        {
            r = rColor.R;
            g = rColor.G;
            b = rColor.B;
        }

        public KTripleColor(byte _r, byte _g, byte _b)
        {
            r = _r;
            g = _g;
            b = _b;
        }

        public static bool operator ==(KTripleColor r, KTripleColor r2) => r != r2;

        public static bool operator !=(KTripleColor r, KTripleColor r2) =>
            r.r == r2.r && r.g == r2.g && r.b == r2.b;

        public byte b, g, r;
    }

    public struct KColor
    {
        public KColor(int argb) => _color = System.Drawing.Color.FromArgb(argb);

        public KColor(KTripleColor rColor, byte _a) => _color = System.Drawing.Color.FromArgb(_a, rColor.r, rColor.g, rColor.b);

        public KColor(byte _r, byte _g, byte _b, byte _a) => _color = System.Drawing.Color.FromArgb(_a, _r, _g, _b);

        System.Drawing.Color _color;

        public int Color => _color.ToArgb();

        public byte A => _color.A;

        public byte R => _color.R;

        public byte G => _color.G;

        public byte B => _color.B;

        public static bool operator ==(KColor c1, KColor c2) => c1._color == c2._color;

        public static bool operator !=(KColor c1, KColor c2) => c1._color != c2._color;

        public static bool operator <(KColor c1, KColor c2) => c1.Color < c2.Color;

        public static bool operator >(KColor c1, KColor c2) => c1.Color > c2.Color;

    }

}
