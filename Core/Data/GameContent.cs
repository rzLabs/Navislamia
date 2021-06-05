using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Configuration;
using Navislamia.Events;
using Navislamia.Maps.Structures;
using Navislamia.X2D;

using Serilog;

namespace Navislamia.Data
{
    public class GameContent
    {
        static ConfigurationManager configMgr = ConfigurationManager.Instance;
        static EventManager eventMgr = EventManager.Instance;

        static int mapWidth = 700000;
        static int mapHeight = 1000000;

        public GameContent()
        {
            mapWidth = configMgr["map_width", "Maps"];
            mapHeight = configMgr["map_height", "Maps"];
        }

        public static void RegisterMapLocationInfo(MapLocationInfo location_info) => QtLocationInfo.Add(location_info);

        public static void RegisterAutoCheckBlockInfo(PolygonF block_info)
        {
            PolygonF blockInfo = new PolygonF(block_info);

            QtAutoBlockInfo.Add(new MapLocationInfo(blockInfo));
        }

        public static bool IsBlocked(float x, float y)
        {
            if (x < 0 || x > mapWidth || y < 0 || y > mapHeight)
                return true;

            if (configMgr["no_collision_check"] != true)
            {
                PointF pt = new PointF(x, y);

                if (QtBlockInfo.Collision(pt))
                    return true;
            }

            return false;
        }

        public static void RegisterBlockInfo(PolygonF block_info)
        {
            PolygonF blockInfo = new PolygonF(block_info);

            QtBlockInfo.Add(new MapLocationInfo(block_info));
        }

        public static void RegisterEventAreaInfo(int eventAreaID, EventAreaInfo event_area)
        {

            if (EventAreaInfo.ContainsKey(eventAreaID))
                EventAreaInfo[eventAreaID] = event_area;
            else
            {
                EventAreaInfo eventArea = new EventAreaInfo(event_area);
                EventAreaInfo.Add(eventAreaID, eventArea);
            }
        }

        public static void RegisterEventAreaBlock(int eventAreaId, PolygonF block)
        {
            if (!EventAreaInfo.ContainsKey(eventAreaId))
                return;

            EventAreaInfo[eventAreaId].Area.Add(block);
        }

        public static bool RegisterPropContactScriptInfo(int propID, int prop_Type, ushort model_info, float x, float y, List<PropContactScriptInfo._FunctionList> functionList)
        {
            if (PropScriptInfo.ContainsKey(propID)) {
                Log.Error("Duplicate prop index: {propID}");
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

        public static QuadTree QtLocationInfo = new QuadTree(0, 0, mapWidth, mapHeight);
        public static QuadTree QtBlockInfo = new QuadTree(0,0, mapWidth, mapHeight);
        public static QuadTree QtAutoBlockInfo = new QuadTree(0, 0, mapWidth, mapHeight);
        public static Dictionary<int, PropContactScriptInfo> PropScriptInfo = new Dictionary<int, PropContactScriptInfo>();
        public static Dictionary<int, EventAreaInfo> EventAreaInfo = new Dictionary<int, EventAreaInfo>();
    }

}
