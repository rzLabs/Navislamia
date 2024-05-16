using Navislamia.Game.Network.Clients;
using Navislamia.Game.World.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.World
{
    public class WorldObject
    {
        protected bool IsDeleted = false;
        protected byte Layer = 0;
        protected WorldMoveVector Move = null;
        protected ConnectionInfo ConnectionInfo = null;
        
        public WorldObjectType Type = WorldObjectType.Unknown;

        bool _inWorld, _nearClient, _changingRegion = false;

        int _previousRegionX, _previousRegionY = 0;

        WorldRegion _region = null;

        ulong _listIndex, _activeIndex = 0;

        protected ConnectionInfo _connectionInfo = null;
    }
}
