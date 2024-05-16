using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.World
{
    public static class WorldTime
    {
        private readonly static int _adjust = 0;
        private readonly static int _rate = 1;

        public static uint CurrentTime
        {
            get
            {
                var elapsed = (uint)DateTime.Now.Ticks / 1000000000;

                return (uint)((elapsed / 100000 + _adjust) / _rate);
            }
        }
    }
}
