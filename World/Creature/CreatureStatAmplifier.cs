using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World.Creature
{
    public class CreatureStatAmplifier
    {
        public CreatureStatAmplifier()
        {
            StatID = new VNumber();
            Strength = new VNumber();
            Vitality = new VNumber();
            Dexterity = new VNumber();
            Agility = new VNumber();
            Intelligence = new VNumber();
            Mentality = new VNumber();
            Luck = new VNumber();
        }

        public VNumber StatID;
        public VNumber Strength;
        public VNumber Vitality;
        public VNumber Dexterity;
        public VNumber Agility;
        public VNumber Intelligence;
        public VNumber Mentality;
        public VNumber Luck;
    }
}
