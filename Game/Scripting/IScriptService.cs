using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Scripting
{

    public interface IScriptService
    {
        bool Start();

        void RegisterFunction(string name, Func<object[], int> function);

        int RunString(string script);
    }
}
