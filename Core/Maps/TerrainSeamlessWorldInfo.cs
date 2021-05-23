using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Navislamia.Data;

using static Navislamia.Data.StringUtil;

namespace Navislamia.Maps
{
    public class TerrainSeamlessWorldInfo
    {
        public bool Initialize(string seamlessWorldInfoFileName, bool mapFileCheck = true)
        {
            const string tileLenHeader = "TILE_LENGTH=";
            const string tileCountPerSegmentHeader = "TILECOUNT_PER_SEGMENT=";
            const string segmentCountPerMapHeader = "SEGMENTCOUNT_PER_MAP=";
            const string mapSizeHeader = "MAPSIZE=";
            const string mapFileHeader = "MAPFILE=";
            const string fovHeader = "SETFOV=";
            const string mapLayerHeader = "MAPLAYER=";

            int mapCountX = 0;
            int mapCountY = 0;

            if (!File.Exists(seamlessWorldInfoFileName))
                return false;

            string[] textLines = File.ReadAllLines(seamlessWorldInfoFileName);

            for (int i = 0; i < textLines.Length; i++)
            {
                string line = textLines[i];
                string content = null;

                if (line == "")
                    continue;

                if (line[0] == ';')
                    continue;

                if ((content = GetStringContent(line, tileLenHeader)) != null)
                    tileLength = float.Parse(content);
                else if ((content = GetStringContent(line, tileCountPerSegmentHeader)) != null)
                    tileCountPerSegment = int.Parse(content);
                else if ((content = GetStringContent(line, segmentCountPerMapHeader)) != null)
                    segmentCountPerMap = int.Parse(content);
                else if ((content = GetStringContent(line, fovHeader)) != null)
                    fov = float.Parse(content);
                else if ((content = GetStringContent(line, mapLayerHeader)) != null)
                    mapLayer = int.Parse(content);
                else if ((content = GetStringContent(line, mapSizeHeader)) != null)
                {
                    if (mapCountX != 0 || mapCountY != 0)
                        return false;

                    int commaPos = content.IndexOf(',');

                    if (commaPos == -1)
                        return false;

                    string strMapCountX = content.Substring(0, commaPos);
                    string strMapCountY = content.Substring(commaPos + 1, content.Length - (commaPos + 1));

                    if (string.IsNullOrEmpty(strMapCountX) || string.IsNullOrEmpty(strMapCountY))
                        return false;

                    mapCountX = int.Parse(strMapCountX);
                    mapCountY = int.Parse(strMapCountY);

                    if (mapCountX < 0 || mapCountY < 0)
                        return false;
                }
                else if ((content = GetStringContent(line, mapFileHeader)) != null)
                {
                    if (mapCountX == 0 && mapCountY == 0)
                        return false;

                    string[] contentBlocks = content.Split(',');

                    if (contentBlocks.Length != 5)
                        return false;

                    string strMapX = contentBlocks[0];
                    string strMapY = contentBlocks[1];
                    string strMapLayer = contentBlocks[2];
                    string strMapFileName = contentBlocks[3];
                    string strWorldMap = contentBlocks[4];

                    if (string.IsNullOrEmpty(strMapX) || string.IsNullOrEmpty(strMapY) || string.IsNullOrEmpty(strMapLayer) || string.IsNullOrEmpty(strMapFileName) || string.IsNullOrEmpty(strWorldMap))
                        return false;

                    int mapX = int.Parse(strMapX);
                    int mapY = int.Parse(strMapY);
                    int mapLay = int.Parse(strMapLayer);

                    if (mapX < 0 || mapX >= mapCountX)
                        return false;

                    if (mapY < 0 || mapY > mapCountY)
                        return false;

                    if (mapLay < 0)
                        return false;

                    int worldMapID = int.Parse(strWorldMap);

                    int mapIndex = (mapY * mapCountX) + mapX + mapLay;

                    if (fileNameMap.ContainsKey(mapIndex))
                        return false;

                    FileNameMapInfo mapInfo = new FileNameMapInfo();
                    mapInfo.MapFileName = strMapFileName;
                    mapInfo.WorldID = worldMapID;
                    fileNameMap.Add(mapIndex, mapInfo);
                }
            }

            if (tileLength <= 0 || tileCountPerSegment <= 0 || segmentCountPerMap <= 0)
                return false;

            if (fileNameMap.Count == 0)
                return false;

            sizeMapCount.CX = mapCountX;
            sizeMapCount.CY = mapCountY;

            return true;
        }

        public void Release()
        {
            fileNameMap.Clear();
        }

        public int TileCountPerSegment => tileCountPerSegment;

        public int SegmentCountPerMap => segmentCountPerMap;

        public float TileLength => tileLength;

        public KSize MapCount => sizeMapCount;

        public int MapLayer => mapLayer;

        public string GetMapFileName(int mapPosX, int mapPosY) => getMapFileNameWithExt(mapPosX, mapPosY, mapFileExt);

        public string GetScriptFileName(int mapPosX, int mapPosY) => getMapFileNameWithExt(mapPosX, mapPosY, scriptFileExt);

        public string GetLocationFileName(int mapPosX, int mapPosY) => getMapFileNameWithExt(mapPosX, mapPosY, locationFileExt);

        public string GetAttributePolygonFileName(int mapPosX, int mapPosY) => getMapFileNameWithExt(mapPosX, mapPosY, attributePolygonFileExt);

        public string GetFieldPropFileName(int mapPosX, int mapPosY) => getMapFileNameWithExt(mapPosX, mapPosY, propFileExt);

        public string GetMinimapImageFileName(int mapPosX, int mapPosY) => getMapFileNameWithExt(mapPosX, mapPosY, minimapFileExt);

        public string GetLQWaterFileName(int mapPosX, int mapPosY) => getMapFileNameWithExt(mapPosX, mapPosY, lqWaterFileExt);

        public string GetEventAreaFileName(int mapPosX, int mapPosY) => getMapFileNameWithExt(mapPosX, mapPosY, eventAreaPolygonFileExt);

        public int GetWorldID(int mapPosX, int mapPosY)
        {
            int mapKey = (mapPosY * sizeMapCount.CX) + mapPosX;

            if (!fileNameMap.ContainsKey(mapKey))
                return -1;

            FileNameMapInfo mapInfo = fileNameMap[mapKey];

            return mapInfo.WorldID;
        }

        public float GetFOV => fov;

        int tileCountPerSegment = 0;
        int segmentCountPerMap = 0;
        float tileLength = 0;
        KSize sizeMapCount = new KSize();
        int mapLayer = 0;
        Dictionary<int, FileNameMapInfo> fileNameMap = new Dictionary<int, FileNameMapInfo>();
        float fov;

        const string mapFileExt = ".nfm";
        const string scriptFileExt = ".nfs";
        const string locationFileExt = ".nfc";
        const string attributePolygonFileExt = ".nfa";
        const string minimapFileExt = ".bmp";
        const string lqWaterFileExt = ".nfw";
        const string propFileExt = ".qpf";
        const string eventAreaPolygonFileExt = ".nfe";

        string getMapFileNameWithExt(int mapPosX, int mapPosY, string ext)
        {
            int mapKey = (mapPosY * sizeMapCount.CX) + mapPosX;

            if (!fileNameMap.ContainsKey(mapKey))
                return null;

            string fileName;

            FileNameMapInfo mapInfo = fileNameMap[(mapPosY * sizeMapCount.CX) + mapPosX];

            fileName = $"{mapInfo.MapFileName}{ext}";

            return fileName;
        }

        ~TerrainSeamlessWorldInfo() => Release();
    }

    public struct FileNameMapInfo
    {
        public string MapFileName;
        public int WorldID;
    }
}
