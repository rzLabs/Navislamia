using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Maps;

namespace Navislamia.LUA.Functions
{
    public static class MiscFunc
    {
        public static int SetCurrentLocationID(params object[] args)
        {
            MapLoader.CurrentLocationID = 0;

            if (args.Length == 0)
                return 0;

            int n = Convert.ToInt32(args[0]);

            MapLoader.CurrentLocationID = n;

            return 0;

        }
    }
}
