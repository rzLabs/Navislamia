using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Command.Interfaces
{
    public interface IGetter
    {
        public dynamic Get(string key, string category);
    }
}
