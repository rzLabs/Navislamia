using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Events;
using Navislamia.Maps.Structures;
using Navislamia.X2D;

namespace Navislamia.Data
{
    public class GameContent
    {
        static EventManager eventMgr = EventManager.Instance;

        const int mapWidth = 700000;
        const int mapHeight = 1000000;

        public static void RegisterMapLocationInfo(MapLocationInfo location_info) { QtLocationInfo.Add(location_info); }

        public static void RegisterAutoCheckBlockInfo(PolygonF block_info)
        {
            PolygonF blockInfo = new PolygonF(block_info);

            QtAutoBlockInfo.Add(new MapLocationInfo(blockInfo));
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

        internal static bool RegisterPropContactScriptInfo(int propID, int prop_Type, ushort model_info, float x, float y, List<PropContactScriptInfo._FunctionList> functionList)
        {
            if (PropScriptInfo.ContainsKey(propID))
                eventMgr.OnException(new ExceptionArgs(new Exception("Duplicate prop index!")));

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
