using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    public interface IScriptingService
    {
        public int ScriptCount { get; set; }

        public int Init(string path = null);

        public int RunString(string script);

        public void RegisterFunction(string name, Func<object[], int> function);
    }
}
