using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.World.Repositories.Entities
{
    public class LevelExp
    {
        public int Level;

        public long Exp;

        public int[] JP { get; set; } = new int[4];
    }
}
