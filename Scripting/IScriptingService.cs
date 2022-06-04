using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    public interface IScriptingService
    {
        public int ScriptCount { get; set; }

        public bool Initialize();

        public int RunString(string script);

        public void RegisterFunction(string name, Func<object[], int> function);
    }
}
