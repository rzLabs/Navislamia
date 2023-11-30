using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Scripting.Functions;

public static class MiscFunc
{
    public static int SetCurrentLocationID(params object[] args)
    {
        // TODO:
        // MapLoader.CurrentLocationID = 0;

        if (args.Length == 0)
            return 0;

        int n = Convert.ToInt32(args[0]);

        // TODO:
        //MapLoader.CurrentLocationID = n;

        return 0;

    }

    public static int GetEnv(params object[] args)
    {
        //var config = configSVC.Get<T>(CATEGORY, KEY, DEFAULTVALUE)

        // return config;

        return 0;
    }
}
