using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Navislamia.Configuration;
using Navislamia.Data;
using static Navislamia.Data.ScriptDefine;
using Navislamia.Events;
using Navislamia.LUA;
using Navislamia.Maps.Structures;
using Navislamia.X2D;

using Serilog;

namespace Navislamia.Maps
{
    public static class MapLoader
    {
        static ConfigurationManager configMgr = ConfigurationManager.Instance;
        static LuaManager luaMgr = LuaManager.Instance;

        static ILogger logger = Log.ForContext(typeof(MapLoader));

        public static void SetDefaultLocation(int x, int y, float mapLength, int locationID)
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

            GameContent.RegisterMapLocationInfo(location_info);
        }

        public static bool Initialize(string directory)
        {
            List<Task> tasks = new List<Task>();
            Task worker = null;

            bool skipNFA = configMgr["skip_loading_nfa", "Maps"];

            tasks.Add(Task.Run(() => {
                seamlessWorldInfo.Initialize($"{directory}\\TerrainSeamlessWorld.cfg");
                logger.Debug("TerrainSeamlessWorld.cfg loaded!");
            }));

            tasks.Add(Task.Run(() => {
                propInfo.Initialize($"{directory}\\TerrainPropInfo.cfg");
                logger.Debug("TerrainPropInfo.cfg loaded!");
            }));

            worker = Task.WhenAll(tasks);
            worker.Wait();

            if (!worker.IsCompletedSuccessfully)
            {
                foreach (Task t in tasks)
                    if (t.IsFaulted)
                        Log.Error(t.Exception, "The MapLoader worker encountered an exception!");

                return false;
            }
            else
            {
                tasks.Clear();
                worker = null;
            }

            tileSize = seamlessWorldInfo.TileLength;
            mapCount = seamlessWorldInfo.MapCount;

            float mapLength = seamlessWorldInfo.TileLength * seamlessWorldInfo.SegmentCountPerMap * seamlessWorldInfo.TileCountPerSegment;
            float attrLen = seamlessWorldInfo.TileLength / (float)attrCountPerTile;

            for (int y = 0; y < mapCount.CY; ++y)
            {
                for (int x = 0; x < mapCount.CX; ++x)
                {
                    Log.Debug("Loading map assets for: m{0}_{1}...", x.ToString("D3"), y.ToString("D3"));

                    string locationFileName = seamlessWorldInfo.GetLocationFileName(x, y);

                    if (string.IsNullOrEmpty(locationFileName))
                        continue;

                    if (seamlessWorldInfo.GetWorldID(x, y) != -1)
                        SetDefaultLocation(x, y, mapLength, seamlessWorldInfo.GetWorldID(x, y));

                    tasks.Add(Task.Run(() =>{
                        LoadLocationFile($"{directory}\\{locationFileName}", x, y, attrLen, mapLength);
                    }));           

                    string scriptFileName = seamlessWorldInfo.GetScriptFileName(x, y);

                    if (string.IsNullOrEmpty(scriptFileName))
                        continue;

                    tasks.Add(Task.Run(() => {
                        LoadScriptFile($"{directory}\\{scriptFileName}", x, y, attrLen, mapLength, propInfo);
                    }));

                    if (!skipNFA)
                    {
                        string attributeFileName = seamlessWorldInfo.GetAttributePolygonFileName(x, y);

                        if (string.IsNullOrEmpty(attributeFileName))
                            continue;

                        tasks.Add(Task.Run(() => {
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
                                Log.Error(t.Exception, "The MapLoader worker encountered an exception!");

                        return false;
                    }
                }
            }

            logger.Information("Map loading complete!");

            return true;
        }

        public static void LoadAttributeFile(string fileName, int x, int y, float attrLen, float mapLength)
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
                    GameContent.RegisterBlockInfo(block_info);

                    if (ptCnt < 50)
                        GameContent.RegisterAutoCheckBlockInfo(block_info);
                }
            }

            Log.Debug("\t- {0} collision polygons loaded", polygonCnt);
        }

        public static void LoadEventAreaFile(string fileName, int x, int y, float attrLen, float mapLength)
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
                        GameContent.RegisterEventAreaBlock(eventAreaID, block_info);
                }
            }
        }

        public static void LoadLocationFile(string fileName, int x, int y, float attrLen, float mapLength)
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
                    luaMgr.RunString(script);
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

                    GameContent.RegisterMapLocationInfo(location_info);
                }
            }

            Log.Debug("\t- {0} location polygons loaded!", localSize);
        }

        public static void LoadScriptFile(string fileName, int x, int y, float attrLen, float mapLength, TerrainPropInfo propInfo)
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

            if (header.Sign != NFSFILE_SIGN) { 
                Log.Fatal("\t- Invalid script header!");
                return;
            }

            if (header.Version != NFSCurrentVer) { 
                Log.Fatal("\t- Invalid script version!");
                return;
            }

            stream.Seek(header.EventLocationOffset, SeekOrigin.Begin);
            loadRegion(stream, x, y, mapLength);

            stream.Seek(header.EventScriptOffset, SeekOrigin.Begin);
            loadRegionScriptInfo(stream, scriptEvents);

            stream.Seek(header.PropScriptOffset, SeekOrigin.Begin);
            loadPropScriptInfo(propInfo, stream, x, y, mapLength);

            currentRegionIdx = regionList.Count;

            logger.Debug("\t- {0} spawn areas loaded!", currentRegionIdx);
        }

        static void loadRegion(KStream stream, int x, int y, float mapLength)
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

        static void loadRegionScriptInfo(KStream stream, List<ScriptRegionInfo> v)
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

        static void loadFunctionInfo(KStream stream, List<ScriptTag> v)
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

        static void loadPropScriptInfo(TerrainPropInfo terrainPropInfo, KStream stream, int x, int y, float mapLength)
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

                GameContent.RegisterPropContactScriptInfo(tag.PropID, tag.Prop_Type, model_id, tag.X, tag.Y, functionList);
            }
        }
        
        public static int CurrentLocationID = 0;
        const int attrCountPerTile = 8;
        static float tileSize = 1;
        static KSize mapCount = new KSize(0, 0);
        static int currentRegionIdx = 0;

        static TerrainSeamlessWorldInfo seamlessWorldInfo = new TerrainSeamlessWorldInfo();
        static TerrainPropInfo propInfo = new TerrainPropInfo();

        static List<ScriptRegion> regionList = new List<ScriptRegion>();
        static List<ScriptRegionInfo> scriptEvents = new List<ScriptRegionInfo>();
    }

}