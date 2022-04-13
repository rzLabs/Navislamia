using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Command.Interfaces
{
    public interface IConfigurationCreator
    {
        public void Create(string path = null);
    }
}
