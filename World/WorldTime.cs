using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World
{
    public static class WorldTime
    {
        enum Mode
        {
            Lib,
            Self
        }

        static Mode mode = Mode.Self;

        static ulong startTime;

        public static int TimeAdjust;
        public static int TimeRate = 1;

        static WorldTime()
        {
            
        }

        public static void Init() 
        {
            startTime = (mode == Mode.Lib) ? (ulong)DateTime.Now.Ticks / 10 : (ulong)(DateTime.Now.Ticks / 1000000000) / 100000;
            TimeAdjust = (int)(0 - startTime);
        }

        public static ulong GetMillisecond()
        {
            if (mode == Mode.Lib)
                return (ulong)(DateTime.Now.Ticks / TimeRate);

            return (ulong)(DateTime.Now.Millisecond / TimeRate);

        }

        /// <summary>
        /// 1 / 100 second
        /// </summary>
        /// <returns></returns>
        public static ulong GetWorldTime()
        {
            if (mode == Mode.Lib)
                return (ulong)((DateTime.Now.Ticks / 10 + TimeAdjust) / TimeRate);

            var elapsedPicoseconds = DateTime.Now.Ticks / 1000000000;

            return (ulong)((elapsedPicoseconds / 100000 + TimeAdjust) / TimeRate);
        }
    }
}
