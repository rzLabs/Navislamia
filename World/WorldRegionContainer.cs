using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static Navislamia.World.WorldOption;

namespace Navislamia.World
{
    public class WorldRegionContainer
    {
        public const int RegionBlockCount = 100;

        public WorldRegionContainer(float mapWidth, float mapHeight)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;

            RegionWidth = (uint)(MapWidth / RegionSize + 1);
            RegionHeight = (uint)(MapHeight / RegionSize + 1);

            RegionBlockWidth = RegionWidth / RegionBlockCount + 1;
            RegionBlockHeight = RegionHeight / RegionBlockCount + 1;

            InitRegion();
        }

        ~WorldRegionContainer()
        {
            DeinitRegion();
        }

        public bool IsValidPosition(float x, float y) => (x < 0 || y < 0 || x >= MapWidth || y >= MapHeight);

        public bool IsValidPosition(WorldPosition pos) => IsValidPosition(pos.X, pos.Y);

        public bool IsValidRegion(uint rx, uint ry, byte layer = 0)
        {
            if ((int)layer >= MaxLayer)
                return false;

            if (rx >= RegionWidth || ry >= RegionHeight)
                return false;

            return true;
        }

        public WorldRegion IfClientExistRegion(uint rx, uint ry, byte layer = 0)
        {
            WorldRegion region = IfExistRegion(rx, ry, layer);

            if (region is null)
                return null;

            if (region.ClientCount > 0)
                return region;

            return null;
        }

        public WorldRegion IfExistRegion(uint rx, uint ry, byte layer = 0)
        {
            if (!IsValidRegion(rx, ry, layer))
                return null;

            return GetRegionPtr(rx, ry, layer);
        }

        public WorldRegion GetRegion(uint rx, uint ry, byte layer = 0)
        {
            if (!IsValidRegion(rx, ry, layer))
                return null;

            return getRegion(rx, ry, layer);
        }

        public WorldRegion GetRegion(float x, float y, byte layer = 0) => GetRegion(GetRegionX(x), GetRegionY(y), layer);

        public WorldRegion GetRegion(WorldPosition pos, byte layer) => GetRegion(pos.X, pos.Y, layer);

        public WorldRegion GetRegion(WorldObject obj) => GetRegion(obj.RX, obj.RY, obj.Layer);

        public void DoEachSpecificRegion(uint rx, uint ry, uint range, byte layer, Func<object, int> functor)
        {
            int left, right, top, bottom;

            left = Math.Max((int)(rx - range), 0);
            right = Math.Min((int)(rx + range), (int)RegionWidth - 1);
            top = Math.Max((int)(ry + range), 0);
            bottom = Math.Min((int)(ry + range), (int)RegionHeight - 1);

            for (uint y = (uint)top; y <= bottom; y++)
                for (uint x = (uint)left; x <= right; x++)
                    if (IsVisibleRegion(rx, ry, x, (uint)y))
                    {
                        WorldRegion region = GetRegionPtr(x, y, layer);

                        if (region is not null)
                            functor(region);
                    }
        }

        public void DoEachNewRegion(uint rx, uint ry, uint prx, uint pry, byte layer, Func<object, int> functor)
        {
            int left = Math.Max(0, Math.Min((int)rx - VisibleRegionRange, (int)rx - VisibleRegionRange));
            int top = Math.Max(0, Math.Min((int)pry - VisibleRegionRange, (int)pry - VisibleRegionRange));

            int right = Math.Min(Math.Max((int)rx + VisibleRegionRange, (int)prx + VisibleRegionRange), (int)RegionWidth - 1);
            int bottom = Math.Min(Math.Max((int)ry + VisibleRegionRange, (int)pry + VisibleRegionRange), (int)RegionHeight - 1);

            bool isCurrentPos, isPrevPos;

            for (uint y = (uint)top; y <= bottom; y++)
                for (uint x = (uint)left; x <= right; x++)
                {
                    isCurrentPos = IsVisibleRegion(rx, ry, x, y);
                    isPrevPos = IsVisibleRegion(prx, pry, x, y);

                    if (isCurrentPos && !isPrevPos)
                    {
                        WorldRegion region = GetRegionPtr(x, y, layer);

                        functor(region);
                    }
                }
        }

        public void DoEachVisibleRegion(uint rx1, uint ry1, uint rx2, uint ry2, byte layer, Func<object, int> functor)
        {
            int left = Math.Max(0, Math.Min((int)rx1 - VisibleRegionRange, (int)rx2 - VisibleRegionRange));
            int top = Math.Max(0, Math.Min((int)ry1 - VisibleRegionRange, (int)ry2 - VisibleRegionRange));

            int right = Math.Min(Math.Max((int)rx1 + VisibleRegionRange, (int)rx2 + VisibleRegionRange), (int)RegionWidth - 1);
            int bottom = Math.Min(Math.Max((int)ry1 + VisibleRegionRange, (int)ry2 + VisibleRegionRange), (int)RegionHeight - 1);


            int pos1, pos2;

            WorldRegion region;

            for (uint y = (uint)top; y <= bottom; y++)
                for (uint x = (uint)left; x <= right; x++)
                {
                    pos1 = Convert.ToInt32(IsVisibleRegion(rx1, ry1, x, y));
                    pos2 = Convert.ToInt32(IsVisibleRegion(rx2, ry2, x, y));

                    if (pos1 == 0 || (pos1 == 0 && pos2 > 0))
                    {
                        region = GetRegionPtr(x, y, layer);

                        if (region is not null)
                            functor(region);
                    }
                }
        }

        public void DoEachRegion(uint rx1, uint ry1, uint rx2, uint ry2, byte layer, Func<object, int> functor)
        {
            int left = Math.Max(0, Math.Min((int)rx1 - VisibleRegionRange, (int)rx2 - VisibleRegionRange));
            int top = Math.Max(0, Math.Min((int)ry1 - VisibleRegionRange, (int)ry2 - VisibleRegionRange));

            int right = Math.Min(Math.Max((int)rx1 + VisibleRegionRange, (int)rx2 + VisibleRegionRange), (int)RegionWidth - 1);
            int bottom = Math.Min(Math.Max((int)ry1 + VisibleRegionRange, (int)ry2 + VisibleRegionRange), (int)RegionHeight - 1);

            WorldRegion region;

            for (uint y = (uint)top; y <= bottom; y++)
                for (uint x = (uint)left; x <= right; x++)
                {
                    region = GetRegionPtr(x, y, layer);

                    if (region is not null)
                        functor(region);
                }
        }

        public class WorldRegionBlock
        {
            bool isLocked = false;

            public WorldRegionBlock()
            {
                Lock.Enter(ref isLocked);

                if (isLocked)
                {
                    for (int i = 0; i < MaxLayer; i++)
                        Region[i] = null;

                    Lock.Exit();
                }
            }

            ~WorldRegionBlock()
            {
                Lock.TryEnter(1000, ref isLocked);

                if (isLocked)
                {
                    for (int i = 0; i < MaxLayer; i++)
                        if (Region[i] is not null)
                            Region = null;

                    Lock.Exit();
                }
            }

            public WorldRegion GetRegionPtr(uint rx , uint ry, byte layer)
            {
                Lock.TryEnter(1000, ref isLocked);

                if (isLocked)
                {
                    if (Region[layer] is null)
                        return null;

                    Lock.Exit();

                    return Region[layer][ry * RegionBlockCount + rx];
                }

                return null;
            }

            public WorldRegion GetRegion(uint rx, uint ry, byte layer)
            {
                Lock.TryEnter(1000, ref isLocked);

                if (isLocked)
                {
                    var regionIdx = ry * RegionBlockCount + rx;

                    if (Region?[layer]?[regionIdx] is not null)
                        return Region[layer][regionIdx];

                    if (Region[layer] is null)
                    {
                        Region[layer] = new WorldRegion[RegionBlockCount * RegionBlockCount];

                        for (int i = 0; i < Region[layer].Length; i++)
                            Region[layer][i] = null;
                    }

                    Region[layer][regionIdx] = new WorldRegion();
                    Region[layer][regionIdx].X = rx;
                    Region[layer][regionIdx].Y = ry;

                    Lock.Exit();

                    return Region[layer][regionIdx];
                }

                return null;
            }

            public WorldRegion[][] Region;
            public SpinLock Lock;
        }

        public void InitRegion()
        {
            Lock.TryEnter(1000, ref isLocked);

            if (isLocked)
            {
                var length = RegionBlockWidth * RegionBlockHeight;

                RegionBlock = new WorldRegionBlock[length];

                for (int i = 0; i < length; i++)
                    RegionBlock[i] = null;

                Lock.Exit();
            }
        }

        public void DeinitRegion()
        {
            Lock.TryEnter(1000, ref isLocked);

            if (isLocked)
            {
                RegionBlock = null;

                Lock.Exit();
            }
        }

        public WorldRegionBlock GetRegionBlockPtr(uint rcx, uint rcy) => RegionBlock[rcy * RegionBlockWidth + rcx];

        public WorldRegionBlock GetRegionBlock(uint rcx, uint rcy)
        {
            Lock.TryEnter(1000, ref isLocked);

            if (isLocked)
            {
                var regionIdx = rcy * RegionBlockWidth + rcx;

                if (RegionBlock[regionIdx] is not null)
                    return RegionBlock[regionIdx];

                RegionBlock[regionIdx] = new WorldRegionBlock();

                Lock.Exit();
            }

            return null;
        }

        public WorldRegion GetRegionPtr(uint rx, uint ry, byte layer)
        {
            uint rcx = rx / RegionBlockCount;
            uint rcy = ry / RegionBlockCount;

            WorldRegionBlock block = GetRegionBlockPtr(rcx, rcy);

            if (block is null)
                return null;

            return block.GetRegionPtr(rx % RegionBlockCount, ry % RegionBlockCount, layer);
        }

        WorldRegion getRegion(uint rx, uint ry, byte layer)
        {
            uint rcx = rx / RegionBlockCount;
            uint rcy = ry / RegionBlockCount;

            WorldRegionBlock block = GetRegionBlock(rcx, rcy);

            return block.GetRegion(rx % RegionBlockCount, ry % RegionBlockCount, layer);
        }

        public float MapWidth, MapHeight;
        public uint RegionWidth;
        public uint RegionHeight;

        public uint RegionBlockWidth;
        public uint RegionBlockHeight;

        public WorldRegionBlock[] RegionBlock;
        public SpinLock Lock;

        bool isLocked = false;
    }
}
