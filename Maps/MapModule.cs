using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Reflection;

using Configuration;
using static Maps.Structures.ScriptDefine;
using Scripting;
using Maps.Structures;
using Maps.X2D;
using Notification;
using Objects;

using Serilog.Events;
using System;
using Autofac;

namespace Maps
{
    public class MapModule : IMapService
    {
        IConfigurationService configSVC;
        IScriptingService scriptSVC;
        INotificationService notificationSVC;

        static int mapWidth = 700000;
        static int mapHeight = 1000000;

        public static QuadTree QtLocationInfo = new QuadTree(0, 0, mapWidth, mapHeight);
        public static QuadTree QtBlockInfo = new QuadTree(0, 0, mapWidth, mapHeight);
        public static QuadTree QtAutoBlockInfo = new QuadTree(0, 0, mapWidth, mapHeight);
        public static Dictionary<int, PropContactScriptInfo> PropScriptInfo = new Dictionary<int, PropContactScriptInfo>();
        public static Dictionary<int, EventAreaInfo> EventAreaInfo = new Dictionary<int, EventAreaInfo>();

        public MapModule(List<object> dependencies)
        {
            if (dependencies.Count < 3)
                return;

            configSVC = dependencies.Find(d => d is IConfigurationService) as IConfigurationService;
            notificationSVC = dependencies.Find(d => d is INotificationService) as INotificationService;
            scriptSVC = dependencies.Find(d => d is IScriptingService) as IScriptingService;
        }

        public void SetDefaultLocation(int x, int y, float mapLength, int locationID)
        {
            MapLocationInfo location_info = new MapLocationInfo();

            PointF[] pt = new PointF[] { new PointF(0, 0), new PointF(0, 0), new PointF(0, 0), new PointF(0, 0) };
            pt[0].Set(x * mapLength, y * mapLength);
            pt[1].Set((x + 1) * mapLength, y * mapLength);
            pt[2].Set((x + 1) * mapLength, (y + 1) * mapLength);
            pt[3].Set(x * mapLength, (y + 1) * mapLength);

            location_info.Priority = 0x7ffffffe;
            location_info.LocationID = locationID;

            location_info.Set(pt);

            registerMapLocationInfo(location_info);
        }

        public bool Initialize(string directory)
        {
            List<Task> tasks = new List<Task>();
            Task worker = null;

            bool skipNFA = configSVC.Get<bool>("skip_loading_nfa", "Maps", false);

            tasks.Add(Task.Run(() =>
            {
                seamlessWorldInfo.Initialize($"{directory}\\TerrainSeamlessWorld.cfg");
                notificationSVC.WriteMarkup("[green]TerrainSeamlessWorld.cfg loaded![/]", LogEventLevel.Information);
            }));

            tasks.Add(Task.Run(() =>
            {
                propInfo.Initialize($"{directory}\\TerrainPropInfo.cfg");
                notificationSVC.WriteMarkup("[green]TerrainPropInfo.cfg loaded![/]", LogEventLevel.Information);
            }));

            worker = Task.WhenAll(tasks);
            worker.Wait();

            if (!worker.IsCompletedSuccessfully)
            {
                foreach (Task t in tasks)
                    if (t.IsFaulted)
                        notificationSVC.WriteException(t.Exception);

                return false;
            }
            else
            {
                tasks.Clear();
                worker = null;
            }

            tileSize = seamlessWorldInfo.TileLength;
            MapCount = seamlessWorldInfo.MapCount;

            float mapLength = seamlessWorldInfo.TileLength * seamlessWorldInfo.SegmentCountPerMap * seamlessWorldInfo.TileCountPerSegment;
            float attrLen = seamlessWorldInfo.TileLength / (float)attrCountPerTile;

            for (int y = 0; y < MapCount.CY; ++y)
            {
                for (int x = 0; x < MapCount.CX; ++x)
                {
                    notificationSVC.WriteMarkup($"Loading map assets for: m{x.ToString("D3")}_{y.ToString("D3")}...", LogEventLevel.Debug);

                    string locationFileName = seamlessWorldInfo.GetLocationFileName(x, y);

                    if (string.IsNullOrEmpty(locationFileName))
                        continue;

                    if (seamlessWorldInfo.GetWorldID(x, y) != -1)
                        SetDefaultLocation(x, y, mapLength, seamlessWorldInfo.GetWorldID(x, y));

                    tasks.Add(Task.Run(() =>
                    {
                        LoadLocationFile($"{directory}\\{locationFileName}", x, y, attrLen, mapLength);
                    }));

                    string scriptFileName = seamlessWorldInfo.GetScriptFileName(x, y);

                    if (string.IsNullOrEmpty(scriptFileName))
                        continue;

                    tasks.Add(Task.Run(() =>
                    {
                        LoadScriptFile($"{directory}\\{scriptFileName}", x, y, attrLen, mapLength, propInfo);
                    }));

                    if (!skipNFA)
                    {
                        string attributeFileName = seamlessWorldInfo.GetAttributePolygonFileName(x, y);

                        if (string.IsNullOrEmpty(attributeFileName))
                            continue;

                        tasks.Add(Task.Run(() =>
                        {
                            LoadAttributeFile($"{directory}\\{attributeFileName}", x, y, attrLen, mapLength);
                        }));
                    }

                    // TODO: Uncomment this once db loading has been implemented
                    /*string eventAreaFileName = seamlessWorldInfo.GetEventAreaFileName(x, y);

                    if (string.IsNullOrEmpty(eventAreaFileName))
                        continue;

                    tasks.Add(Task.Run(() => { 
                        LoadEventAreaFile($"{directory}\\{eventAreaFileName}", x, y, attrLen, mapLength);
                        logger.Debug("- Loaded event areas");
                    }))*/

                    worker = Task.WhenAll(tasks);
                    worker.Wait();

                    if (!worker.IsCompletedSuccessfully)
                    {
                        foreach (Task t in tasks)
                            if (t.IsFaulted)
                                notificationSVC.WriteException(t.Exception);

                        return false;
                    }
                }
            }

            return true;
        }

        public void LoadAttributeFile(string fileName, int x, int y, float attrLen, float mapLength)
        {
            if (!File.Exists(fileName))
                return;

            KStream stream = new KStream(fileName);

            if (stream == null)
                return;

            int polygonCnt = stream.ReadInt();

            PolygonF block_info = new PolygonF();

            for (int i = 0; i < polygonCnt; ++i)
            {
                int ptCnt = stream.ReadInt();

                if (ptCnt < 3)
                    continue;

                PointF[] points = new PointF[ptCnt];

                for (int pointNum = 0; pointNum < points.Length; ++pointNum)
                    points[pointNum] = new PointF(stream.ReadInt(), stream.ReadInt());

                for (int pointNum = 0; pointNum < points.Length; ++pointNum)
                {
                    points[pointNum].X = mapLength * x + points[pointNum].X * attrLen;
                    points[pointNum].Y = mapLength * y + points[pointNum].Y * attrLen;
                }

                if (block_info.Set(points))
                {
                    QtBlockInfo.Add(new MapLocationInfo(new PolygonF(block_info)));

                    if (ptCnt < 50)
                        QtAutoBlockInfo.Add(new MapLocationInfo(new PolygonF(block_info)));
                }
            }

            notificationSVC.WriteMarkup($"[orange3]\t- {polygonCnt} collision polygons loaded[/]",  LogEventLevel.Debug);
        }

        public void LoadEventAreaFile(string fileName, int x, int y, float attrLen, float mapLength)
        {
            if (!File.Exists(fileName))
                return;

            KStream stream = new KStream(fileName);

            if (stream == null)
                return;

            int eventAreaCount = stream.ReadInt();

            for (int eventAreaIdx = 0; eventAreaIdx < eventAreaCount; ++eventAreaIdx)
            {
                int eventAreaID = stream.ReadInt();
                int polygonCnt = stream.ReadInt();

                PolygonF block_info = new PolygonF();

                for (int i = 0; i < polygonCnt; ++i)
                {
                    int ptCnt = stream.ReadInt();

                    PointF[] points = new PointF[ptCnt];

                    for (int pointNum = 0; pointNum < points.Length; ++pointNum)
                        points[pointNum] = new PointF(stream.ReadInt(), stream.ReadInt());

                    for (int pointNum = 0; pointNum < points.Length; ++pointNum)
                    {
                        points[pointNum].X = mapLength * x + points[pointNum].X * attrLen;
                        points[pointNum].Y = mapLength * y + points[pointNum].Y * attrLen;
                    }

                    if (block_info.Set(points))
                        registerEventAreaBlock(eventAreaID, block_info);
                }
            }
        }

        private void registerEventAreaBlock(int eventAreaID, PolygonF block_info)
        {
            if (!EventAreaInfo.ContainsKey(eventAreaID))
                return;

            EventAreaInfo[eventAreaID].Area.Add(block_info);
        }

        public void LoadLocationFile(string fileName, int x, int y, float attrLen, float mapLength)
        {
            MapLocationInfo location_info = new MapLocationInfo();

            if (!File.Exists(fileName))
                return;

            KStream stream = new KStream(fileName);

            if (stream == null)
                return;

            int localSize = 0;

            localSize = stream.ReadInt();

            for (int localCount = 0; localCount < localSize; ++localCount)
            {
                LocationInfoHeader locationInfoHeader = new LocationInfoHeader();

                locationInfoHeader.Priority = stream.ReadInt();
                locationInfoHeader.CenterPosition = new Point3D
                {
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat(),
                    Z = stream.ReadFloat()
                };
                locationInfoHeader.Radius = stream.ReadFloat();

                location_info.Priority = locationInfoHeader.Priority;

                int charSize = stream.ReadInt();

                if (charSize > 1)
                {
                    string localName = stream.ReadString(charSize);
                }

                charSize = stream.ReadInt();

                CurrentLocationID = 0;

                if (charSize > 1)
                {
                    string script = stream.ReadString(charSize);
                    scriptSVC.RunString(script);
                }

                if (CurrentLocationID == 0)
                    return;

                location_info.LocationID = CurrentLocationID;

                int polygonSize = stream.ReadInt();

                for (int polygonCount = 0; polygonCount < polygonSize; ++polygonCount)
                {
                    int pointCount = 0;
                    location_info.Clear();

                    pointCount = stream.ReadInt();

                    Point[] points = new Point[pointCount];
                    PointF[] pt = new PointF[pointCount];

                    for (int pointNum = 0; pointNum < pointCount; ++pointNum)
                    {
                        points[pointNum] = new Point()
                        {
                            X = stream.ReadInt(),
                            Y = stream.ReadInt()
                        };
                    }

                    for (int pointNum = 0; pointNum < pointCount; ++pointNum)
                        pt[pointNum] = new PointF(
                            mapLength * x + points[pointNum].X * attrLen,
                            mapLength * y + points[pointNum].Y * attrLen);

                    location_info.Set(pt);

                    registerMapLocationInfo(location_info);
                }
            }

            notificationSVC.WriteMarkup("[orange3]\t- {0} location polygons loaded![/]", LogEventLevel.Debug);
        }

        private void registerMapLocationInfo(MapLocationInfo location_info) => QtLocationInfo.Add(location_info);

        public void LoadScriptFile(string fileName, int x, int y, float attrLen, float mapLength, TerrainPropInfo propInfo)
        {
            if (!File.Exists(fileName))
                return;

            KStream stream = new KStream(fileName);

            if (stream == null)
                return;

            NFS_HEADER_V02 header = new NFS_HEADER_V02()
            {
                Sign = stream.ReadString(16),
                Version = stream.ReadInt(),
                EventLocationOffset = stream.ReadInt(),
                EventScriptOffset = stream.ReadInt(),
                PropScriptOffset = stream.ReadInt()
            };

            if (header.Sign != NFSFILE_SIGN)
            {
                notificationSVC.WriteMarkup("[bold red]\t- Invalid script header![/]", LogEventLevel.Fatal);
                return;
            }

            if (header.Version != NFSCurrentVer)
            {
                notificationSVC.WriteMarkup("[red\\t- Invalid script version![/]", LogEventLevel.Fatal);
                return;
            }

            stream.Seek(header.EventLocationOffset, SeekOrigin.Begin);
            loadRegion(stream, x, y, mapLength);

            stream.Seek(header.EventScriptOffset, SeekOrigin.Begin);
            loadRegionScriptInfo(stream, scriptEvents);

            stream.Seek(header.PropScriptOffset, SeekOrigin.Begin);
            loadPropScriptInfo(propInfo, stream, x, y, mapLength);

            currentRegionIdx = regionList.Count;

            notificationSVC.WriteMarkup($"[orange3]\t- {currentRegionIdx} spawn areas loaded![/]", LogEventLevel.Debug);
        }

        void loadRegion(KStream stream, int x, int y, float mapLength)
        {
            int locationCount = 0;
            float sx = x * mapLength;
            float sy = y * mapLength;

            locationCount = stream.ReadInt();

            for (int i = 0; i < locationCount; ++i)
            {
                ScriptRegion sr = new ScriptRegion()
                {
                    Left = (tileSize * ((float)stream.ReadInt() + sx)),
                    Top = (tileSize * ((float)stream.ReadInt() + sy)),
                    Right = (tileSize * ((float)stream.ReadInt() + sx)),
                    Bottom = (tileSize * ((float)stream.ReadInt() + sy)),
                };

                int length = stream.ReadInt();

                if (length > 0)
                    sr.Name = stream.ReadString(length);

                regionList.Add(sr);
            }
        }

        void loadRegionScriptInfo(KStream stream, List<ScriptRegionInfo> v)
        {
            int scriptCount = 0;

            scriptCount = stream.ReadInt();

            for (int i = 0; i < scriptCount; ++i)
            {
                ScriptRegionInfo tag = new ScriptRegionInfo(0);

                tag.RegionIndex = stream.ReadInt();

                loadFunctionInfo(stream, tag.InfoList);

                for (int j = 0; j < tag.InfoList.Count; ++j)
                {
                    ScriptTag st = tag.InfoList[j];

                    string left;
                    string right;
                    string top;
                    string bottom;
                    string box;

                    if (st.Function.IndexOf("#") != -1)
                    {
                        left = regionList[currentRegionIdx + tag.RegionIndex].Left.ToString();
                        right = regionList[currentRegionIdx + tag.RegionIndex].Right.ToString();
                        top = regionList[currentRegionIdx + tag.RegionIndex].Top.ToString();
                        bottom = regionList[currentRegionIdx + tag.RegionIndex].Bottom.ToString();
                        box = $"{regionList[currentRegionIdx + tag.RegionIndex].Left},{regionList[currentRegionIdx + tag.RegionIndex].Top},{regionList[currentRegionIdx + tag.RegionIndex].Right},{regionList[currentRegionIdx + tag.RegionIndex].Bottom}";

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

        void loadFunctionInfo(KStream stream, List<ScriptTag> v)
        {
            int functionCount = 0;
            ScriptTag info;

            functionCount = stream.ReadInt();

            for (int i = 0; i < functionCount; ++i)
            {
                info = new ScriptTag();

                info.Trigger = stream.ReadInt();

                int length = stream.ReadInt();

                if (length > 0)
                    info.Function = stream.ReadString(length);

                v.Add(info);
            }
        }

        void loadPropScriptInfo(TerrainPropInfo terrainPropInfo, KStream stream, int x, int y, float mapLength)
        {
            int scriptCount = stream.ReadInt();

            for (int i = 0; i < scriptCount; ++i)
            {
                PropContactScriptInfo tag;

                ushort model_id = 0;

                tag = new PropContactScriptInfo()
                {
                    PropID = stream.ReadInt(),
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat()
                };

                model_id = stream.ReadUShort();

                tag.X += x * mapLength;
                tag.Y += y * mapLength;

                if (terrainPropInfo.GetPropType(model_id) == PropType.NPC)
                    tag.Prop_Type = (int)PropContactScriptInfo.PropType.NPC;
                else if (terrainPropInfo.GetPropType(model_id) == PropType.USE_NX3)
                    tag.Prop_Type = (int)PropContactScriptInfo.PropType.Prop;

                List<ScriptTag> tagList = new List<ScriptTag>();
                loadFunctionInfo(stream, tagList);

                List<PropContactScriptInfo._FunctionList> functionList = new List<PropContactScriptInfo._FunctionList>();

                for (int j = 0; j < tagList.Count; ++j)
                {
                    PropContactScriptInfo._FunctionList tmp = new PropContactScriptInfo._FunctionList();

                    if (tagList[j].Trigger == (int)ScriptTrigger.Initialize)
                        tmp.TriggerID = (int)PropContactScriptInfo._FunctionList.TriggerType.INIT;
                    else if (tagList[j].Trigger == (int)ScriptTrigger.Contact)
                        tmp.TriggerID = (int)PropContactScriptInfo._FunctionList.TriggerType.CONTACT;

                    tmp.Function = tagList[j].Function;

                    functionList.Add(tmp);
                }

                registerPropContactScriptInfo(tag.PropID, tag.Prop_Type, model_id, tag.X, tag.Y, functionList);
            }
        }

        private bool registerPropContactScriptInfo(int propID, int prop_Type, ushort model_info, float x, float y, List<PropContactScriptInfo._FunctionList> functionList)
        {
            if (PropScriptInfo.ContainsKey(propID))
            {
                notificationSVC.WriteMarkup($"[bold orange3]Duplicate prop index: {propID}[/]", LogEventLevel.Warning);
            }

            PropContactScriptInfo tag = new PropContactScriptInfo()
            {
                PropID = propID,
                X = x,
                Y = y,
                Model_Info = model_info,
                FunctionList = functionList,
                Prop_Type = prop_Type
            };

            PropScriptInfo.Add(propID, tag);

            return true;
        }

        public static int CurrentLocationID = 0;
        const int attrCountPerTile = 8;
        static float tileSize = 1;
        public KSize MapCount { get; set; } = new KSize(0, 0);
        static int currentRegionIdx = 0;

        static TerrainSeamlessWorldInfo seamlessWorldInfo = new TerrainSeamlessWorldInfo();
        static TerrainPropInfo propInfo = new TerrainPropInfo();

        static List<ScriptRegion> regionList = new List<ScriptRegion>();
        static List<ScriptRegionInfo> scriptEvents = new List<ScriptRegionInfo>();
    }

}