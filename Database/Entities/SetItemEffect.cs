using Navislamia.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Database.Entities
{
    public class SetItemEffect
    {
        public const int MAX_OPTIONS = 4;

        public int SetID;

        public int EffectID;

        public int SetPartID;

        public short[] BaseType { get; set; } = new short[MAX_OPTIONS];

        public VNumber[] BaseVar1 { get; set; } = new VNumber[MAX_OPTIONS];

        public VNumber[] BaseVar2 { get; set; } = new VNumber[MAX_OPTIONS];

        public short[] OptType { get; set; } = new short[MAX_OPTIONS];

        public VNumber[] OptVar1 { get; set; } = new VNumber[MAX_OPTIONS];

        public VNumber[] OptVar2 { get; set;} = new VNumber[MAX_OPTIONS];

        public List<EffectInfo> Effects { get; set; } = new List<EffectInfo>();

    }
}
