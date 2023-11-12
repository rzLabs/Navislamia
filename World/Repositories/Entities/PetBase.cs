using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World.Repositories.Entities
{
    public class PetBase
    {
        enum PET_RATE_FLAG
        {
            RARE = 1 << 0,
            SHOVELABLE = 1 << 1,
            COLLECTABLE = 1 << 2
        }

        public int UID;
        public int Type;
        public int NameID;
        public int CageID;
        public byte Rate;
        public float Size;
        public float Scale;

        public int LocalFlag;
    }
}
