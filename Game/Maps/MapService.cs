using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using Navislamia.Configuration.Options;
using Navislamia.Game.Maps.Constants;
using Navislamia.Game.Maps.Entities;
using Navislamia.Game.Maps.Enums;
using Navislamia.Game.Maps.X2D;
using Navislamia.Game.Scripting;

using Navislamia.Game.Filer;

using static Navislamia.Game.Maps.Entities.ScriptDefine;

using Serilog;

namespace Navislamia.Game.Maps
{
    public class MapService : IMapService
    {
        private const int AttrCountPerTile = 8;

        private readonly MapOptions _mapOptions;
        private readonly IScriptService _scriptService;

        private static QuadTree _qtLocationInfo;
        private static QuadTree _qtBlockInfo;
        private static QuadTree _qtAutoBlockInfo;
        private static Dictionary<int, PropContactScriptInfo> _propScriptInfo;
        private static Dictionary<int, EventAreaInfo> _eventAreaInfo; // TODO: currently only updated but never used

        private static int _currentLocationId;
        private static float _tileSize = 1;
        public KSize MapCount { get; set; } = new(0, 0);
        private static int _currentRegionIdx;

        private static readonly TerrainSeamlessWorldInfo SeamlessWorldInfo = new();
        private static readonly TerrainPropInfo PropInfo = new();
        private static readonly List<ScriptRegion> RegionList = new();
        private static readonly List<ScriptRegionInfo> ScriptEvents = new();

        private readonly ILogger _logger = Log.ForContext<MapService>();

        public MapService(IOptions<MapOptions> mapOptions, IScriptService scriptContent)
        {
            _mapOptions = mapOptions.Value;
            _scriptService = scriptContent;

            _qtLocationInfo = new QuadTree(0, 0, _mapOptions.Width, _mapOptions.Height);
            _qtBlockInfo = new QuadTree(0, 0, _mapOptions.Width, _mapOptions.Height);
            _qtAutoBlockInfo = new QuadTree(0, 0, _mapOptions.Width, _mapOptions.Height);
            _propScriptInfo = new Dictionary<int, PropContactScriptInfo>();
            _eventAreaInfo = new Dictionary<int, EventAreaInfo>();
        }

        public void Start(string directory)
        {
            List<Task> tasks = new();

            var skipLoadingNfa = _mapOptions.SkipLoadingNfa;

            tasks.Add(Task.Run(() =>
            {
                SeamlessWorldInfo.Initialize($"{directory}\\TerrainSeamlessWorld.cfg");
                _logger.Debug("{fileName} loaded!", "TerrainSeamlessWorld.cfg");
            }));

            tasks.Add(Task.Run(() =>
            {
                PropInfo.Initialize($"{directory}\\TerrainPropInfo.cfg");
                _logger.Debug("{fileName} loaded!", "TerrainPropInfo.cfg");
            }));

            try
            {
                var worker = Task.WhenAll(tasks);
                worker.Wait();

                if (!worker.IsCompletedSuccessfully)
                {
                    foreach (var t in tasks.Where(t => t.IsFaulted))
                    {
                        _logger.Error("A task has failed {exception}", t.Exception?.Message); // TODO: should include stack trace
                        throw new Exception();
                    }
                    return;
                }

                tasks.Clear();

                _tileSize = SeamlessWorldInfo.TileLength;
                MapCount = SeamlessWorldInfo.SizeMapCount;

                var mapLength = SeamlessWorldInfo.TileLength * SeamlessWorldInfo.SegmentCountPerMap * SeamlessWorldInfo.TileCountPerSegment;
                var attrLen = SeamlessWorldInfo.TileLength / AttrCountPerTile;

                for (var y = 0; y < MapCount.CY; ++y)
                {
                    for (var x = 0; x < MapCount.CX; ++x)
                    {
                        _logger.Debug("Loading map: m{x}_{y}...", x.ToString("D3"), y.ToString("D3"));

                        var locationFileName = SeamlessWorldInfo.GetLocationFileName(x, y);

                        if (string.IsNullOrEmpty(locationFileName))
                        {
                            continue;
                        }

                        if (SeamlessWorldInfo.GetWorldId(x, y) != -1)
                        {
                            SetDefaultLocation(x, y, mapLength, SeamlessWorldInfo.GetWorldId(x, y));
                        }

                        tasks.Add(Task.Run(() =>
                        {
                            LoadLocationFile($"{directory}\\{locationFileName}", x, y, attrLen, mapLength);
                        }));

                        var scriptFileName = SeamlessWorldInfo.GetScriptFileName(x, y);

                        if (string.IsNullOrEmpty(scriptFileName))
                        {
                            continue;
                        }

                        tasks.Add(Task.Run(() =>
                        {
                            LoadScriptFile($"{directory}\\{scriptFileName}", x, y, attrLen, mapLength, PropInfo);
                        }));

                        if (!skipLoadingNfa)
                        {
                            var attributeFileName = SeamlessWorldInfo.GetAttributePolygonFileName(x, y);

                            if (string.IsNullOrEmpty(attributeFileName))
                            {
                                continue;
                            }

                            tasks.Add(Task.Run(() =>
                            {
                                LoadAttributeFile($"{directory}\\{attributeFileName}", x, y, attrLen, mapLength);
                            }));
                        }

                        var eventAreaFileName = SeamlessWorldInfo.GetEventAreaFileName(x, y);

                        if (string.IsNullOrEmpty(eventAreaFileName))
                        {
                            continue;
                        }

                        tasks.Add(Task.Run(() =>
                        {
                            LoadEventAreaFile($"{directory}\\{eventAreaFileName}", x, y, attrLen, mapLength);
                        }));

                        worker = Task.WhenAll(tasks);
                        worker.Wait();

                        if (worker.IsCompletedSuccessfully)
                        {
                            continue;
                        }

                        foreach (var t in tasks.Where(t => t.IsFaulted))
                        {
                            _logger.Error("A task has failed {exception}", t.Exception?.Message); // TODO: should include stack trace
                            throw new Exception();
                        }

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to load maps! {exception}", ex); // TODO: needs to include stack trace
                return;
            }

            _logger.Debug("{mapCount} Maps loaded successfully!", MapCount.CX + MapCount.CY);
        }

        private void SetDefaultLocation(int x, int y, float mapLength, int locationId)
        {
            var locationInfo = new MapLocationInfo();

            PointF[] points = { new(0, 0), new(0, 0), new(0, 0), new(0, 0) };
            points[0].Set(x * mapLength, y * mapLength);
            points[1].Set((x + 1) * mapLength, y * mapLength);
            points[2].Set((x + 1) * mapLength, (y + 1) * mapLength);
            points[3].Set(x * mapLength, (y + 1) * mapLength);

            locationInfo.Priority = 0x7ffffffe;
            locationInfo.LocationId = locationId;
            locationInfo.Set(points);

            RegisterMapLocationInfo(locationInfo);
        }

        private void LoadAttributeFile(string fileName, int x, int y, float attrLen, float mapLength)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            KStream stream = new(fileName);

            var polygonCnt = stream.ReadInt();

            PolygonF blockInfo = new();

            for (var i = 0; i < polygonCnt; ++i)
            {
                var pointCount = stream.ReadInt();

                if (pointCount < 3)
                    continue;

                var points = new PointF[pointCount];

                for (var pointNum = 0; pointNum < points.Length; ++pointNum)
                {
                    points[pointNum] = new PointF(stream.ReadInt(), stream.ReadInt());
                }

                foreach (var point in points)
                {
                    point.X = mapLength * x + point.X * attrLen;
                    point.Y = mapLength * y + point.Y * attrLen;
                }

                if (!blockInfo.Set(points))
                {
                    continue;
                }

                _qtBlockInfo.Add(new MapLocationInfo(new PolygonF(blockInfo)));

                if (pointCount < 50)
                {
                    _qtAutoBlockInfo.Add(new MapLocationInfo(new PolygonF(blockInfo)));
                }
            }
        }

        private void LoadEventAreaFile(string fileName, int x, int y, float attrLen, float mapLength)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            KStream stream = new(fileName);

            var eventAreaCount = stream.ReadInt();

            for (var eventAreaIdx = 0; eventAreaIdx < eventAreaCount; ++eventAreaIdx)
            {
                var eventAreaId = stream.ReadInt();
                var polygonCnt = stream.ReadInt();

                for (var i = 0; i < polygonCnt; ++i)
                {
                    var pointCount = stream.ReadInt();

                    var points = new PointF[pointCount];

                    for (var pointNum = 0; pointNum < points.Length; ++pointNum)
                    {
                        points[pointNum] = new PointF(stream.ReadInt(), stream.ReadInt());
                    }

                    foreach (var point in points)
                    {
                        point.X = mapLength * x + point.X * attrLen;
                        point.Y = mapLength * y + point.Y * attrLen;
                    }

                    _eventAreaInfo[eventAreaId] = new EventAreaInfo(eventAreaId, points);
                }
            }
        }

        private void LoadLocationFile(string fileName, int x, int y, float attrLen, float mapLength)
        {
            MapLocationInfo locationInfo = new();

            if (!File.Exists(fileName))
            {
                return;
            }

            var stream = new KStream(fileName);

            var localSize = stream.ReadInt();

            for (var localCount = 0; localCount < localSize; ++localCount)
            {
                var locationInfoHeader = new LocationInfoHeader
                {
                    Priority = stream.ReadInt(),
                    CenterPosition = new Point3D
                    {
                        X = stream.ReadFloat(),
                        Y = stream.ReadFloat(),
                        Z = stream.ReadFloat()
                    },
                    Radius = stream.ReadFloat()
                };

                locationInfo.Priority = locationInfoHeader.Priority;

                var charSize = stream.ReadInt();

                if (charSize > 1)
                {
                    // TODO localName never used?
                    var localName = stream.ReadString(charSize);
                }

                charSize = stream.ReadInt();

                _currentLocationId = 0;

                if (charSize > 1)
                {
                    var script = stream.ReadString(charSize);
                    _scriptService.RunString(script);
                }

                if (_currentLocationId == 0)
                {
                    return;
                }

                locationInfo.LocationId = _currentLocationId;

                var polygonSize = stream.ReadInt();

                for (var polygonCount = 0; polygonCount < polygonSize; ++polygonCount)
                {
                    locationInfo.ClearPolygon();

                    var pointCount = stream.ReadInt();
                    var points = new Point[pointCount];
                    var point = new PointF[pointCount];

                    for (var pointNum = 0; pointNum < pointCount; ++pointNum)
                    {
                        points[pointNum] = new Point
                        {
                            X = stream.ReadInt(),
                            Y = stream.ReadInt()
                        };
                    }

                    for (var pointNum = 0; pointNum < pointCount; ++pointNum)
                        point[pointNum] = new PointF(
                            mapLength * x + points[pointNum].X * attrLen,
                            mapLength * y + points[pointNum].Y * attrLen);

                    locationInfo.Set(point);

                    RegisterMapLocationInfo(locationInfo);
                }
            }
        }

        private void RegisterMapLocationInfo(MapLocationInfo locationInfo) => _qtLocationInfo.Add(locationInfo);

        private void LoadScriptFile(string fileName, int x, int y, float attrLen, float mapLength, TerrainPropInfo terrainPropInfo)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            var stream = new KStream(fileName);

            var header = new NFS_HEADER_V02()
            {
                Sign = stream.ReadString(16),
                Version = stream.ReadInt(),
                EventLocationOffset = stream.ReadInt(),
                EventScriptOffset = stream.ReadInt(),
                PropScriptOffset = stream.ReadInt()
            };

            if (header.Sign != ScriptDefineConstants.NFSFILE_SIGN)
            {
                _logger.Error("\t- Invalid script header!\n"); // TODO: may need more info
                return;
            }

            if (header.Version != ScriptDefineConstants.NFSCurrentVer)
            {
                _logger.Error("\t- Invalid script version!\n"); // TODO: may need more info
                return;
            }

            stream.Seek(header.EventLocationOffset, SeekOrigin.Begin);
            LoadRegion(stream, x, y, mapLength);

            stream.Seek(header.EventScriptOffset, SeekOrigin.Begin);
            LoadRegionScriptInfo(stream, ScriptEvents);

            stream.Seek(header.PropScriptOffset, SeekOrigin.Begin);
            LoadPropScriptInfo(terrainPropInfo, stream, x, y, mapLength);

            _currentRegionIdx = RegionList.Count;
        }

        private void LoadRegion(KStream stream, int x, int y, float mapLength)
        {
            var sx = x * mapLength;
            var sy = y * mapLength;

            var locationCount = stream.ReadInt();

            for (var i = 0; i < locationCount; ++i)
            {
                var sr = new ScriptRegion
                {
                    Left = _tileSize * (stream.ReadInt() + sx),
                    Top = _tileSize * (stream.ReadInt() + sy),
                    Right = _tileSize * (stream.ReadInt() + sx),
                    Bottom = _tileSize * (stream.ReadInt() + sy),
                };

                var length = stream.ReadInt();

                if (length > 0)
                {
                    sr.Name = stream.ReadString(length);
                }

                RegionList.Add(sr);
            }
        }

        private void LoadRegionScriptInfo(KStream stream, List<ScriptRegionInfo> v)
        {
            var scriptCount = stream.ReadInt();

            for (var i = 0; i < scriptCount; ++i)
            {
                var tag = new ScriptRegionInfo
                {
                    RegionIndex = stream.ReadInt()
                };

                LoadFunctionInfo(stream, tag.InfoList);

                for (var j = 0; j < tag.InfoList.Count; ++j)
                {
                    var st = tag.InfoList[j];

                    if (st.Function.IndexOf("#", StringComparison.Ordinal) != -1)
                    {
                        var left = RegionList[_currentRegionIdx + tag.RegionIndex].Left.ToString(CultureInfo.InvariantCulture);
                        var right = RegionList[_currentRegionIdx + tag.RegionIndex].Right.ToString(CultureInfo.InvariantCulture);
                        var top = RegionList[_currentRegionIdx + tag.RegionIndex].Top.ToString(CultureInfo.InvariantCulture);
                        var bottom = RegionList[_currentRegionIdx + tag.RegionIndex].Bottom.ToString(CultureInfo.InvariantCulture);
                        var box =
                            $"{RegionList[_currentRegionIdx + tag.RegionIndex].Left}," +
                            $"{RegionList[_currentRegionIdx + tag.RegionIndex].Top}," +
                            $"{RegionList[_currentRegionIdx + tag.RegionIndex].Right}," +
                            $"{RegionList[_currentRegionIdx + tag.RegionIndex].Bottom}";

                        st.Function = st.Function.Replace("#LEFT", left);
                        st.Function = st.Function.Replace("#RIGHT", right);
                        st.Function = st.Function.Replace("#TOP", top);
                        st.Function = st.Function.Replace("#BOTTOM", bottom);
                        st.Function = st.Function.Replace("#BOX", box);
                        st.Function = st.Function.Replace("#box", box);
                    }

                    tag.InfoList[j] = st;
                }

                v.Add(tag);
            }
        }

        private void LoadFunctionInfo(KStream stream, List<ScriptTag> v)
        {
            var functionCount = stream.ReadInt();

            for (var i = 0; i < functionCount; ++i)
            {
                var info = new ScriptTag
                {
                    Trigger = stream.ReadInt()
                };

                var length = stream.ReadInt();

                if (length > 0)
                    info.Function = stream.ReadString(length);

                v.Add(info);
            }
        }

        private void LoadPropScriptInfo(TerrainPropInfo terrainPropInfo, KStream stream, int x, int y, float mapLength)
        {
            var scriptCount = stream.ReadInt();

            for (var i = 0; i < scriptCount; ++i)
            {
                var tag = new PropContactScriptInfo
                {
                    PropId = stream.ReadInt(),
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat()
                };

                var modelId = stream.ReadUShort();

                tag.X += x * mapLength;
                tag.Y += y * mapLength;

                if (terrainPropInfo.GetPropType(modelId) == PropType.NPC)
                {
                    tag.PropType = PropContractType.NPC;
                }
                else if (terrainPropInfo.GetPropType(modelId) == PropType.USE_NX3)
                {
                    tag.PropType = PropContractType.Prop;
                }

                var tagList = new List<ScriptTag>();
                LoadFunctionInfo(stream, tagList);

                var functionList = new List<FunctionList>();

                for (var j = 0; j < tagList.Count; ++j)
                {
                    var tmp = new FunctionList();

                    tmp.TriggerId = tagList[j].Trigger switch
                    {
                        (int)ScriptTrigger.Initialize => TriggerType.INIT,
                        (int)ScriptTrigger.Contact => TriggerType.CONTACT,
                        _ => tmp.TriggerId
                    };

                    tmp.Function = tagList[j].Function;

                    functionList.Add(tmp);
                }

                RegisterPropContactScriptInfo(tag.PropId, tag.PropType, modelId, tag.X, tag.Y, functionList);
            }
        }

        private void RegisterPropContactScriptInfo(int propId, PropContractType propType, ushort modelInfo, float x, float y,
            List<FunctionList> functionList)
        {
            if (_propScriptInfo.ContainsKey(propId))
            {
                _logger.Warning("Duplicate prop index: {propId}", propId);
                return;
            }

            var tag = new PropContactScriptInfo
            {
                PropId = propId,
                X = x,
                Y = y,
                ModelInfo = modelInfo,
                FunctionList = functionList,
                PropType = propType
            };

            _propScriptInfo.Add(propId, tag);
        }
    }
}

