using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Navislamia.Configuration;
using Navislamia.Data;
using static Navislamia.Data.ScriptDefine;
using Navislamia.Events;
using Navislamia.LUA;
using Navislamia.Maps.Structures;
using Navislamia.X2D;

namespace Navislamia.Maps
{
    public static class MapLoader
    {
        static EventManager eventMgr = EventManager.Instance;
        static ConfigurationManager configMgr = ConfigurationManager.Instance;
        static LuaManager luaMgr = LuaManager.Instance;

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
            bool result = false;
            bool skipNFA = configMgr["skip_loading_nfa", "Maps"];

            TerrainSeamlessWorldInfo seamlessWorldInfo = new TerrainSeamlessWorldInfo();
            result = seamlessWorldInfo.Initialize($"{directory}\\TerrainSeamlessWorld.cfg");

            if (result) { } // TODO: send to logging service
                //eventMgr.OnMessage(new MessageArgs("- TerrainSeamlessWorldInfo.cfg loaded!"));
            else
            {
                // TODO: should also be reported to the logging service?
                eventMgr.OnException(new ExceptionArgs(new Exception("TerrainSeamlessWorldInfo.cfg read error!")));
                return false;
            }

            TerrainPropInfo propInfo = new TerrainPropInfo();
            result = propInfo.Initialize($"{directory}\\TerrainPropInfo.cfg");

            if (result) { } // TODO: send to logging service
                //eventMgr.OnMessage(new MessageArgs("- TerrainPropInfo.cfg loaded!"));
            else
            {
                // TODO: should also be reported to the logging service?
                eventMgr.OnException(new ExceptionArgs(new Exception("TerrainPropInfo.cfg read error!")));
                return false;
            }

            tileSize = seamlessWorldInfo.TileLength;

            KSize mapCount = seamlessWorldInfo.MapCount;

            float mapLength = seamlessWorldInfo.TileLength * seamlessWorldInfo.SegmentCountPerMap * seamlessWorldInfo.TileCountPerSegment;
            float attrLen = seamlessWorldInfo.TileLength / (float)attrCountPerTile;

            for (int y = 0; y < mapCount.CY; ++y)
            {
                for (int x = 0; x < mapCount.CX; ++x)
                {
                    string locationFileName = seamlessWorldInfo.GetLocationFileName(x, y);

                    if (string.IsNullOrEmpty(locationFileName))
                        continue;

                    if (seamlessWorldInfo.GetWorldID(x, y) != -1)
                        SetDefaultLocation(x, y, mapLength, seamlessWorldInfo.GetWorldID(x, y));

                    // TODO: send to logging service
                    //eventMgr.OnMessage(new MessageArgs($"- Loading region areas {locationFileName}"));

                    LoadLocationFile($"{directory}\\{locationFileName}", x, y, attrLen, mapLength);              

                    string scriptFileName = seamlessWorldInfo.GetScriptFileName(x, y);

                    if (string.IsNullOrEmpty(scriptFileName))
                        continue;

                    // TODO: send to logging service
                    //eventMgr.OnMessage(new MessageArgs($"- Loading spawn areas: {scriptFileName}"));

                    LoadScriptFile($"{directory}\\{scriptFileName}", x, y, attrLen, mapLength, propInfo);

                    if (!skipNFA)
                    {
                        string attributeFileName = seamlessWorldInfo.GetAttributePolygonFileName(x, y);

                        if (string.IsNullOrEmpty(attributeFileName))
                            continue;

                        // TODO: send to logging service
                        //eventMgr.OnMessage(new MessageArgs($"- Loading collision areas: {attributeFileName}"));

                        LoadAttributeFile($"{directory}\\{attributeFileName}", x, y, attrLen, mapLength);
                    }

                    // TODO: this shouldn't be enabled until db loading has completed!
                    /*string eventAreaFileName = seamlessWorldInfo.GetEventAreaFileName(x, y);

                    if (string.IsNullOrEmpty(eventAreaFileName))
                        continue;

                    LoadEventAreaFile($"{directory}\\{eventAreaFileName}", x, y, attrLen, mapLength);*/
                }
            }

            eventMgr.OnMessage(new MessageArgs("- Map loading complete!"));      

            return false;
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

            if (header.Sign != NFSFILE_SIGN)
                eventMgr.OnException(new ExceptionArgs(new Exception("Invalid script (nfs) header!")));

            if (header.Version != NFSCurrentVer)
                eventMgr.OnException(new ExceptionArgs(new Exception("Invalid script (nfs) version!")));

            stream.Seek(header.EventLocationOffset, SeekOrigin.Begin);
            LoadRegionInfo(stream, x, y, mapLength);

            stream.Seek(header.EventScriptOffset, SeekOrigin.Begin);
            LoadRegionScriptInfo(stream, scriptEvents);

            stream.Seek(header.PropScriptOffset, SeekOrigin.Begin);
            LoadPropScriptInfo(propInfo, stream, x, y, mapLength);

            currentRegionIdx = regionList.Count;
        }

        public static void LoadRegionInfo(KStream stream, int x, int y, float mapLength)
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

        public static void LoadRegionScriptInfo(KStream stream, List<ScriptRegionInfo> v)
        {
            int scriptCount = 0;

            scriptCount = stream.ReadInt();

            for (int i = 0; i < scriptCount; ++i)
            {
                ScriptRegionInfo tag = new ScriptRegionInfo(0);

                tag.RegionIndex = stream.ReadInt();

                LoadFunctionInfo(stream, tag.InfoList);

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

        public static void LoadFunctionInfo(KStream stream, List<ScriptTag> v)
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

        public static void LoadPropScriptInfo(TerrainPropInfo terrainPropInfo, KStream stream, int x, int y, float mapLength)
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
                LoadFunctionInfo(stream, tagList);

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
        static int currentRegionIdx = 0;

        static List<ScriptRegion> regionList = new List<ScriptRegion>();
        static List<ScriptRegionInfo> scriptEvents = new List<ScriptRegionInfo>();
    }

    public struct ScriptRegion
    {
        public ScriptRegion(int left, int top, int right, int bottom, string name)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            Name = name;
        }

        public float Left, Top, Right, Bottom;
        public string Name;
    }

    public struct ScriptTag
    {
        public int Trigger;
        public string Function;
    }

    public struct ScriptRegionInfo
    {
        public ScriptRegionInfo(int regionIndex)
        {
            RegionIndex = regionIndex;
            InfoList = new List<ScriptTag>();
        }

        public int RegionIndex;
        public List<ScriptTag> InfoList;
    }
}