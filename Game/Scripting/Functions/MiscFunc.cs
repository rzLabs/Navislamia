using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Scripting.Functions;

public static class MiscFunc
{
    public static void SetCurrentLocationId(params object[] args)
    {
        // TODO:
        // MapLoader.CurrentLocationID = 0;

        if (args.Length == 0)
        {
            return;
        }

        var n = Convert.ToInt32(args[0]);

        // TODO:
        //MapLoader.CurrentLocationID = n;
    }

    public static int GetEnv(params object[] args)
    {
        //var config = configSVC.Get<T>(CATEGORY, KEY, DEFAULTVALUE)

        // return config;

        return 0;
    }
}
