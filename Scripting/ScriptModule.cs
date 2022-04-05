using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Threading.Tasks;

using Configuration;
using Scripting.Functions;
using Utilities;
using Notification;

using MoonSharp.Interpreter;
using Serilog.Events;

using Spectre.Console;

namespace Scripting
{
    public class ScriptModule : Autofac.Module, IScriptingService
    {
        public string ScriptsDirectory;
        public int ScriptCount { get; set; }

        Script luaVM = new Script();
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        public ScriptModule() { }

        public ScriptModule(IConfigurationService configurationService, INotificationService notificationService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;
        }

        public int Init(string directory = null)
        {
            string scriptDir = directory ?? $"{Directory.GetCurrentDirectory()}\\Scripts\\";

            if (string.IsNullOrEmpty(scriptDir) || !Directory.Exists(scriptDir))
            {
                notificationSVC.WriteMarkup("[bold red]LuaManager failed to initialize because the provided directory was null or does not exist![/]", LogEventLevel.Error);
                return 1;
            }

            ScriptsDirectory = scriptDir;

            registerFunctions();

            loadScripts();

            return 0;
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

        protected override void Load(ContainerBuilder builder)
        {
            var configServiceTypes = Directory.EnumerateFiles(Environment.CurrentDirectory)
                .Where(filename => filename.Contains("Modules") && filename.EndsWith("Scripting.dll"))
                .Select(filepath => Assembly.LoadFrom(filepath))
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(IScriptingService).IsAssignableFrom(type) && type.IsClass));

            foreach (var configServiceType in configServiceTypes)
                builder.RegisterType(configServiceType).As<IScriptingService>();
        }

        void registerFunctions()
        {
            RegisterFunction("call_lc_In", MiscFunc.SetCurrentLocationID);
            RegisterFunction("get_monster_id", MonsterFunc.get_monster_id);
        }

        void loadScripts()
        {
            string[] scriptPaths;

            if (string.IsNullOrEmpty(ScriptsDirectory) || !Directory.Exists(ScriptsDirectory))
            {
                notificationSVC.WriteMarkup("[bold red]ScriptModule failed to load because the scripts directory is null or does not exist![/]", LogEventLevel.Error);
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
                            exMsg = $"{Path.GetFileName(path)} could not be loaded!\n\nMessage: {StringExt.LuaExceptionToString(((InterpreterException)ex).DecoratedMessage)}\n";
                        else
                            exMsg = $"An exception occured while loading {Path.GetFileName(path)}!\n\nMessage: {ex.Message}\nStack-Trace: {ex.StackTrace}\n";

                        notificationSVC.WriteMarkup($"[bold red]{exMsg}[/]");
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
                    notificationSVC.WriteException(task.Exception);
        }
    }
}
