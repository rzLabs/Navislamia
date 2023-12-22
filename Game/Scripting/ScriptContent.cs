using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using Navislamia.Notification;
using Navislamia.Scripting.Functions;
using Navislamia.Utilities;

namespace Navislamia.Game.Scripting
{
    public class ScriptContent
    {
        public string ScriptsDirectory;
        public int ScriptCount { get; set; }

        Script luaVM = new();
        private readonly INotificationModule _notificationModule;

        public static ScriptContent Instance { get; private set; }

        public ScriptContent(INotificationModule notificationModule)
        {
            _notificationModule = notificationModule;

            Instance = this;
        }

        public bool Init(string directory = null)
        {
            try
            {
                string scriptDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Scripts");
                if (string.IsNullOrEmpty(scriptDir) || !Directory.Exists(scriptDir))
                {
                    Directory.CreateDirectory(scriptDir);
                    _notificationModule.WriteWarning("Missing directory: .\\Scripts has been created!");
                }

                ScriptsDirectory = scriptDir;

                registerFunctions();

                loadScripts();

                _notificationModule.WriteSuccess(new[] { "Scripts loaded successfully!", $"[green]{ScriptCount}[/] scripts loaded!" }, true);

            }
            catch (Exception e)
            {
                _notificationModule.WriteError($"Failed loads scripts!");
                _notificationModule.WriteException(e);
                return false;
            }

            return true;
        }

        public void RegisterFunction(string name, Func<object[], int> function) => luaVM.Globals[name] = function;

        public int RunString(string script)
        {

            if (string.IsNullOrEmpty(script))
            {
                //Log.Error("Cannot RunString for a null script!");
                return 0;
            }

            try
            {
                luaVM.DoString(script);
            }
            catch (ScriptRuntimeException rtEx)
            {
                //Log.Error("A runtime exception occured while executing the lua string: {0}\n- Message: {1}\n- Stack-Trace: {2}", script, rtEx.Message, rtEx.StackTrace);
                return 0;
            }
            catch (SyntaxErrorException sEx)
            {
                //Log.Error("A syntax exception occured while executing the lua string: {0}\n- Message: {1}\n- Stack-Trace: {2}", script, sEx.Message, sEx.StackTrace);
                return 0;
            }
            catch (Exception ex)
            {
                //Log.Error("An exception occured while executing the lua string: {0}\n- Message: {1}\n- Stack-Trace: {2}", script, ex.Message, ex.StackTrace);
                return 0;
            }

            return 1;
        }


        // TODO: Sandro, this is where you register your lua functions
        void registerFunctions()
        {
            RegisterFunction("call_lc_In", MiscFunc.SetCurrentLocationID);
            RegisterFunction("get_monster_id", MonsterFunc.get_monster_id);
            RegisterFunction("get_value", Player.get_value);
            //RegisterFunction("get_local", MiscFunc.GetLocal);
        }

        void loadScripts()
        {
            string[] scriptPaths;

            if (string.IsNullOrEmpty(ScriptsDirectory) || !Directory.Exists(ScriptsDirectory))
            {
                _notificationModule.WriteError("ScriptModule failed to load because the scripts directory is null or does not exist!");
                return;
            }

            scriptPaths = Directory.GetFiles(ScriptsDirectory);
            List<Task> scriptTasks = new List<Task>();

            for (int i = 0; i < scriptPaths.Length; i++)
            {
                string path = scriptPaths[i];

                scriptTasks.Add(Task.Run(() =>
                {
                    try
                    {
                        luaVM.DoFile(path);
                    }
                    catch (Exception ex)
                    {

                        string exMsg;

                        if (ex is SyntaxErrorException || ex is ScriptRuntimeException)
                            exMsg = $"{Path.GetFileName(path)} could not be loaded!\n\nMessage: {StringExtensions.LuaExceptionToString(((InterpreterException)ex).DecoratedMessage)}\n";
                        else
                            exMsg = $"An exception occured while loading {Path.GetFileName(path)}!\n\nMessage: {ex.Message}\nStack-Trace: {ex.StackTrace}\n";

                        _notificationModule.WriteMarkup($"[bold red]{exMsg}[/]");
                    }
                }));


                ScriptCount++;
            }

            Task t = Task.WhenAll(scriptTasks);

            try
            {
                t.Wait();
            }
            catch { }

            foreach (var task in scriptTasks)
                if (task.IsFaulted)
                    _notificationModule.WriteException(task.Exception);
        }
    }
}
