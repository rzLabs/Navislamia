using Navislamia.Game.Network;
using Navislamia.Game.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Creature.Entities
{
    public class PlayerEntity : CreatureEntity
    {
        public int Handle { get; set; }

        public IConnection Connection { get; set; }
    }
}
