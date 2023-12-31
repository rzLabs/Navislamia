using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZQuadTree
{
    public struct QuadRect
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Width;
        public readonly int Height;

        public QuadRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int HalfWidth
        {
            get
            {
                return Width / 2;
            }
        }

        public int HalfHeight
        {
            get
            {
                return Height / 2;
            }
        }

        public int CenterX
        {
            get
            {
                return X + HalfWidth;
            }
        }

        public int CenterY
        {
            get
            {
                return Y + HalfHeight;
            }
        }

        public int Top
        {
            get
            {
                return Y;
            }
        }

        public int Left
        {
            get
            {
                return X;
            }
        }

        public int Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        public int Right
        {
            get
            {
                return X + Width;
            }
        }
    }
}
