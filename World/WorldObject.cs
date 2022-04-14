using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World
{
    public class WorldObject
    {


        ulong listIndex;
        ulong activeIndex;

        protected byte WorldObjectType;
        protected dynamic tag;
        protected WorldMoveVector mv;
        protected byte layer;
        protected bool isDeleted;
    }
}
