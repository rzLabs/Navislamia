using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MoonSharp.Interpreter;
using Navislamia.Events;
using Navislamia.LUA.Functions;

namespace Navislamia.LUA
{
    public class LuaManager
    {
        EventManager eventMgr = EventManager.Instance;
        Script luaVM = new Script();

        public static LuaManager Instance;

        public string ScriptsDirectory = null;

        public Exception Exception = null;

        public LuaManager() => Instance = this;

        public bool Initialize(string directory)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException("Directory cannot be null!");

            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException($"Provided directory: {directory} does not exist!");

            ScriptsDirectory = directory;

            registerFunctions();

            Task t = loadScripts();

            t.Wait();

            if (t.IsFaulted)
            {
                Exception = t.Exception;
                return false;
            }
            else
                return true;
        }

        public void RunString(string script) => luaVM.DoString(script);

        void registerFunctions()
        {
            registerFunction("call_lc_In", MiscFunc.SetCurrentLocationID);
            registerFunction("get_monster_id", MonsterFunc.get_monster_id);
        }

        void registerFunction(string name, Func<object[], int> function) => luaVM.Globals[name] = function;

        async Task loadScripts()
        {
            string[] scriptPaths;

            if (string.IsNullOrEmpty(ScriptsDirectory))
                throw new ArgumentNullException("ScriptsDirectory cannot be null!");

            if (!Directory.Exists(ScriptsDirectory))
                throw new DirectoryNotFoundException($"Provided directory: {ScriptsDirectory} does not exist!");

            scriptPaths = Directory.GetFiles(ScriptsDirectory);

             await Task.Run(() => {
                for (int i = 0; i < scriptPaths.Length; i++)
                     luaVM.DoFile(scriptPaths[i]);
             });

            eventMgr.OnMessage(new MessageArgs($"- {scriptPaths.Length} LUA script files loaded!"));
        }
    }
}
