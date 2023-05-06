using System;

namespace Navislamia.Scripting
{
    public interface IScriptingModule
    {
        public int ScriptCount { get; set; }

        public int Init(string directory = null);

        public int RunString(string script);

        public void RegisterFunction(string name, Func<object[], int> function);
    }
}
