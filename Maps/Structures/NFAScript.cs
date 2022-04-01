using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maps.Structures
{
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
