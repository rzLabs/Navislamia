using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Entities;
using Navislamia.World.Entities;

namespace Navislamia.Creature.Entities
{
    public class Creature : GameObject
    {
        CreatureStatAmplifier StateAmplifier = new CreatureStatAmplifier();
    }
}
