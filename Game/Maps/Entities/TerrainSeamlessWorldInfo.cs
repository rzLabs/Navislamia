using System.Collections.Generic;
using System.IO;
using Objects;
using static Navislamia.Utilities.StringExtensions;

namespace Navislamia.Game.Maps.Entities;

public class TerrainSeamlessWorldInfo
{
    private const string TileLenHeader = "TILE_LENGTH=";
    private const string TileCountPerSegmentHeader = "TILECOUNT_PER_SEGMENT=";
    private const string SegmentCountPerMapHeader = "SEGMENTCOUNT_PER_MAP=";
    private const string MapSizeHeader = "MAPSIZE=";
    private const string MapFileHeader = "MAPFILE=";
    private const string FovHeader = "SETFOV=";
    private const string MapLayerHeader = "MAPLAYER=";
    
    public float GetFov { get; private set; }
    public int TileCountPerSegment { get; private set; }
    public int SegmentCountPerMap { get; private set; }
    public float TileLength { get; private set; }
    
    private KSize _sizeSizeMapCount;
    private int _mapLayer;
    private Dictionary<int, FileNameMapInfo> _fileNameMap = new ();

    private const string MapFileExt = ".nfm";
    private const string ScriptFileExt = ".nfs";
    private const string LocationFileExt = ".nfc";
    private const string AttributePolygonFileExt = ".nfa";
    private const string MinimapFileExt = ".bmp";
    private const string LqWaterFileExt = ".nfw";
    private const string PropFileExt = ".qpf";
    private const string EventAreaPolygonFileExt = ".nfe";
    
    // TODO: Check if requried, c# has a garbage collector
    ~TerrainSeamlessWorldInfo() => Release();

    
    public void Initialize(string seamlessWorldInfoFileName, bool mapFileCheck = true)
    {
        var mapCountX = 0;
        var mapCountY = 0;

        if (!File.Exists(seamlessWorldInfoFileName))
        {
            return;
        }

        var textLines = File.ReadAllLines(seamlessWorldInfoFileName);

        foreach (var line in textLines)
        {
            string content;

            if (line == "")
            {
                continue;
            }

            if (line[0] == ';')
            {
                continue;
            }

            if ((content = line.GetStringContent(TileLenHeader)) != null)
            {
                TileLength = float.Parse(content);
            }
            
            if ((content = line.GetStringContent(TileCountPerSegmentHeader)) != null)
            {
                TileCountPerSegment = int.Parse(content);
            }
            
            if ((content = line.GetStringContent(SegmentCountPerMapHeader)) != null)
            {
                SegmentCountPerMap = int.Parse(content);
            }
            
            if ((content = line.GetStringContent(FovHeader)) != null)
            {
                GetFov = float.Parse(content);
            }
            
            if ((content = line.GetStringContent(MapLayerHeader)) != null)
            {
                _mapLayer = int.Parse(content);
            }
            
            if ((content = line.GetStringContent(MapSizeHeader)) != null)
            {
                if (mapCountX != 0 || mapCountY != 0)
                {
                    return;
                }

                var commaPos = content.IndexOf(',');

                if (commaPos == -1)
                {
                    return;
                }

                var strMapCountX = content[..commaPos];
                var strMapCountY = content.Substring(commaPos + 1, content.Length - (commaPos + 1));

                if (string.IsNullOrEmpty(strMapCountX) || string.IsNullOrEmpty(strMapCountY))
                {
                    return;
                }

                mapCountX = int.Parse(strMapCountX);
                mapCountY = int.Parse(strMapCountY);

                if (mapCountX < 0 || mapCountY < 0)
                {
                    return;
                }
            }

            if ((content = line.GetStringContent(MapFileHeader)) == null)
            {
                continue;
            }

            if (mapCountX == 0 && mapCountY == 0)
            {
                return;
            }

            var contentBlocks = content.Split(',');

            if (contentBlocks.Length != 5)
            {
                return;
            }

            var strMapX = contentBlocks[0];
            var strMapY = contentBlocks[1];
            var strMapLayer = contentBlocks[2];
            var strMapFileName = contentBlocks[3];
            var strWorldMap = contentBlocks[4];

            if (string.IsNullOrEmpty(strMapX) || string.IsNullOrEmpty(strMapY) ||
                string.IsNullOrEmpty(strMapLayer) || string.IsNullOrEmpty(strMapFileName) ||
                string.IsNullOrEmpty(strWorldMap)) return;

            var mapX = int.Parse(strMapX);
            var mapY = int.Parse(strMapY);
            var mapLay = int.Parse(strMapLayer);

            if (mapX < 0 || mapX >= mapCountX)
            {
                return;
            }

            if (mapY < 0 || mapY > mapCountY)
            {
                return;
            }

            if (mapLay < 0)
            {
                return;
            }

            var worldMapId = int.Parse(strWorldMap);

            var mapIndex = mapY * mapCountX + mapX + mapLay;

            if (_fileNameMap.ContainsKey(mapIndex))
            {
                return;
            }

            FileNameMapInfo mapInfo = new ()
            {
                MapFileName = strMapFileName,
                WorldId = worldMapId
            };
            _fileNameMap.Add(mapIndex, mapInfo);
        }

        if (TileLength <= 0 || TileCountPerSegment <= 0 || SegmentCountPerMap <= 0)
        {
            return;
        }

        if (_fileNameMap.Count == 0)
        {
            return;
        }

        _sizeSizeMapCount.CX = mapCountX;
        _sizeSizeMapCount.CY = mapCountY;
    }

    private void Release()
    {
        _fileNameMap.Clear();
    }

    public KSize SizeMapCount => _sizeSizeMapCount;

    public string GetMapFileName(int mapPosX, int mapPosY) => GetMapFileNameWithExt(mapPosX, mapPosY, MapFileExt);

    public string GetScriptFileName(int mapPosX, int mapPosY) => GetMapFileNameWithExt(mapPosX, mapPosY, ScriptFileExt);

    public string GetLocationFileName(int mapPosX, int mapPosY) => GetMapFileNameWithExt(mapPosX, mapPosY, LocationFileExt);

    public string GetAttributePolygonFileName(int mapPosX, int mapPosY) => GetMapFileNameWithExt(mapPosX, mapPosY, AttributePolygonFileExt);

    public string GetFieldPropFileName(int mapPosX, int mapPosY) => GetMapFileNameWithExt(mapPosX, mapPosY, PropFileExt);

    public string GetMinimapImageFileName(int mapPosX, int mapPosY) => GetMapFileNameWithExt(mapPosX, mapPosY, MinimapFileExt);

    public string GetLqWaterFileName(int mapPosX, int mapPosY) => GetMapFileNameWithExt(mapPosX, mapPosY, LqWaterFileExt);

    public string GetEventAreaFileName(int mapPosX, int mapPosY) => GetMapFileNameWithExt(mapPosX, mapPosY, EventAreaPolygonFileExt);

    public int GetWorldId(int mapPosX, int mapPosY)
    {
        var mapKey = mapPosY * _sizeSizeMapCount.CX + mapPosX;

        if (!_fileNameMap.ContainsKey(mapKey))
        {
            return -1;
        }

        var mapInfo = _fileNameMap[mapKey];

        return mapInfo.WorldId;
    }

    private string GetMapFileNameWithExt(int mapPosX, int mapPosY, string ext)
    {
        var mapKey = mapPosY * _sizeSizeMapCount.CX + mapPosX;

        if (!_fileNameMap.ContainsKey(mapKey))
        {
            return null;
        }

        var mapInfo = _fileNameMap[mapPosY * _sizeSizeMapCount.CX + mapPosX];
        var fileName = $"{mapInfo.MapFileName}{ext}";

        return fileName;
    }

}