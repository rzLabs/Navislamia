using System;

namespace Navislamia.Game.World
{
    public class WorldRegionContainer
    {
        const int _regionChunkCount = 100;
        const int _regionSize = 150;
        const short _maxLayer = 256;
        const int _regionVisibleRange = 3;
        const int _visibleRegionBoxWidth = 7;

        int[][] _visibleMatrix = new int[_visibleRegionBoxWidth][]
        {
            new []{ 0,0,1,1,1,0,0 },
            new []{ 0,1,1,1,1,1,0 },
            new []{ 1,1,1,1,1,1,1 },
            new []{ 1,1,1,1,1,1,1 },
            new []{ 1,1,1,1,1,1,1 },
            new []{ 0,1,1,1,1,1,0 },
            new []{ 0,0,1,1,1,0,0 }
        };
        

        float _mapWidth, _mapHeight;
        int _regionWidth, _regionHeight, _regionChunkWidth, _regionChunkHeight;

        WorldRegionChunk[] _regionChunk;

        public WorldRegionContainer(float mapWidth, float mapHeight)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _regionWidth = (int)(mapWidth / (_regionSize + 1));
            _regionHeight = (int)(mapHeight / (_regionSize + 1));
            _regionChunkWidth = _regionWidth / _regionChunkCount + 1;
            _regionChunkHeight = _regionHeight / _regionChunkCount + 1;

            _regionChunk = new WorldRegionChunk[_regionChunkWidth * _regionChunkHeight];
        }

        public WorldRegionChunk this[uint regionChunkX, uint regionChunkY]
        {
            get
            {
                return _regionChunk[regionChunkY * _regionChunkWidth + regionChunkX];
            }
        }

        public void DoEachNewRegion(uint regionX, uint regionY, uint previousRegionX, uint previousRegionY, ushort layer, Func<WorldRegion, int> regionFunc)
        {
            int _left, _top, _right, _bottom;

            _left = (int)Math.Max(0, Math.Min(regionX - _regionVisibleRange, previousRegionX - _regionVisibleRange));
            _top = (int)Math.Max(0, Math.Min(regionY - _regionVisibleRange, previousRegionY - _regionVisibleRange));
            _right = (int)Math.Min(Math.Max(regionX + _regionVisibleRange, previousRegionX + _regionVisibleRange), _regionWidth - 1);
            _bottom = (int)Math.Min(Math.Max(regionY + _regionVisibleRange, previousRegionY + _regionVisibleRange), _regionHeight - 1);

            int _isCurrentPosition, _isPreviousPosition;

            for (int y = _top; y <= _bottom; y++)
            {
                for (int x = _left; x <= _right; x++)
                {
                    _isCurrentPosition = RegionIsVisible(regionX, regionY, (uint)x, (uint)y);
                    _isPreviousPosition = RegionIsVisible(previousRegionX, previousRegionY, (uint)x, (uint)y);

                    if (_isCurrentPosition > 0 && _isPreviousPosition == 0)
                    {
                        WorldRegion _region = GetRegion((uint)x, (uint)y, layer);

                        if (_region is not null)
                        {
                            regionFunc(_region);
                        }                        
                    }
                }
            }
        }

        public void DoEachVisibleRegion(uint regionX1, uint regionY1, uint regionX2, uint regionY2, ushort layer, Func<WorldRegion, int> regionFunc)
        {
            int _left, _top, _right, _bottom;

            _left = (int)Math.Max(0, Math.Min(regionX1 - _regionVisibleRange, regionX2 - _regionVisibleRange));
            _top = (int)Math.Max(0, Math.Min(regionY1 - _regionVisibleRange, regionY2 - _regionVisibleRange));
            _right = (int)Math.Min(Math.Max(regionX1 + _regionVisibleRange, regionX2 + _regionVisibleRange), _regionWidth - 1);
            _bottom = (int)Math.Min(Math.Max(regionY1 + _regionVisibleRange, regionY2 + _regionVisibleRange), _regionHeight - 1);

            int _nearestPosition1, _nearestPosition2;

            for (int y = _top; y <= _bottom; y++ )
            {
                for (int x = _left; x <= _right; x++)
                {
                    _nearestPosition1 = RegionIsVisible(regionX1, regionY1, (uint)x, (uint)y);
                    _nearestPosition2 = RegionIsVisible(regionX2, regionY2, (uint)x, (uint)y);

                    if (_nearestPosition1 > 0 || (_nearestPosition1 == 0 && _nearestPosition2 > 0))
                    {
                        WorldRegion _region = GetRegion((uint)x, (uint)y, layer);

                        if (_region is not null)
                        {
                            regionFunc(_region);
                        }
                    }

                }
            }
        }

        public void DoEachRegion(uint regionX1, uint regionY1, uint regionX2, uint regionY2, ushort layer, Func<WorldRegion, int> regionFunc)
        {
            uint _left, _top, _right, _bottom;

            _left = Math.Max(0, Math.Min(regionX1 - _regionVisibleRange, regionX2 - _regionVisibleRange));
            _top = Math.Max(0, Math.Min(regionY1 - _regionVisibleRange, regionY2 - _regionVisibleRange));
            _right = Math.Min(Math.Max(regionX1 + _regionVisibleRange, regionX2 + _regionVisibleRange), (uint)_regionWidth - 1);
            _bottom = Math.Min(Math.Max(regionY1 + _regionVisibleRange, regionY2 + _regionVisibleRange), (uint)_regionHeight - 1);

            for (uint y = _top; y <= _bottom; y++)
            {
                for (uint x = _left; x <= _right; x++)
                {
                    WorldRegion region = GetRegion(x, y, layer);

                    if (region is not null)
                    {
                        regionFunc(region);
                    }
                }
            }
        }

        private WorldRegionChunk GetRegionChunk(uint regionChunkX, uint regionChunkY)
        {
            var regionChunk = _regionChunk[regionChunkY * _regionChunkWidth + regionChunkX];

            if (regionChunk is not null)
            {
                return regionChunk;
            }

            _regionChunk[regionChunkY * _regionChunkWidth + regionChunkX] = new WorldRegionChunk();

            return _regionChunk[regionChunkY * _regionChunkWidth + regionChunkX];
        }

        private bool RegionExists(uint regionX, uint regionY, ushort layer)
        {
            uint _regionChunkX, _regionChunkY;

            _regionChunkX = regionX / _regionChunkCount;
            _regionChunkY = regionY / _regionChunkCount;

            WorldRegionChunk regionChunk = GetRegionChunk(_regionChunkX, _regionChunkY);

            if (regionChunk is null)
                return false;

            WorldRegion region = regionChunk.GetRegion(regionX % _regionChunkCount, regionY % _regionChunkCount, layer);

            if (region is null)
                return false;

            return true;
        }

        private WorldRegion GetRegion(uint regionX, uint regionY, ushort layer)
        {
            uint _regionChunkX, _regionChunkY;

            _regionChunkX = regionX / _regionChunkCount;
            _regionChunkY = regionY / _regionChunkCount;

            WorldRegionChunk regionChunk = GetRegionChunk(_regionChunkX, _regionChunkY);
            WorldRegion region = regionChunk.GetRegion(regionX % _regionChunkCount, regionY % _regionChunkCount, layer);

            return region;
        }

        private int RegionIsVisible(uint regionX, uint regionY, uint x, uint y)
        {
            int _nearestX, _nearestY;

            _nearestX = (int)(_regionVisibleRange + x - regionX);
            _nearestY = (int)(_regionVisibleRange + y - regionY);

            if (_nearestX < 0 || _nearestX >= _regionVisibleRange)
            {
                return 0;
            }

            if (_nearestY < 0 || _nearestY >= _regionVisibleRange)
            {
                return 0;
            }

            return _visibleMatrix[_nearestX][_nearestY];
        }

        public bool ValidPosition(float x, float y)
        {
            if (x < 0 || y < 0)
                return false;

            if (x >= _mapWidth || y >= _mapHeight ) 
                return false;

            return true;
        }

        public bool ValidPosition(WorldPosition position)
        {
            return ValidPosition(position.X, position.Y);
        }

        public bool ValidRegion(uint regionX, uint regionY, short layer = 0)
        {
            if (layer >= _maxLayer)
                return false;

            if (regionX >= _regionWidth || regionY >= _regionHeight)
                return false;

            return true;
        }

        public class WorldRegionChunk
        {
            const ushort _maxLayer = 256; // TODO: should come from config

            WorldRegion[][] _region = new WorldRegion[_maxLayer][];

            public WorldRegionChunk()
            {
                for (int i = 0; i < _maxLayer; i++)
                {
                    _region[i] = null;
                }
            }

            public WorldRegion GetRegion(uint regionX, uint regionY, ushort layer)
            {
                if (_region[layer] is not null)
                {
                    var region = _region[layer][regionY * _regionChunkCount + regionX];

                    if (region is not null)
                        return region;
                }

                if (_region[layer] is null)
                {
                    _region[layer] = new WorldRegion[_regionChunkCount * _regionChunkCount];
                }

                _region[layer][regionY * _regionChunkCount + regionX] = new WorldRegion();
                _region[layer][regionY * _regionChunkCount + regionX].X = regionX;
                _region[layer][regionY * _regionChunkCount + regionX].Y = regionY;
                _region[layer][regionY * _regionChunkCount + regionX].Layer = layer;

                return _region[layer][regionY * _regionChunkCount + regionX];
            }
        }
    }
}
