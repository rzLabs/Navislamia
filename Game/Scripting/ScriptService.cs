using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MoonSharp.Interpreter;
using Navislamia.Game.Scripting.Functions;
using Navislamia.Scripting.Functions;

namespace Navislamia.Game.Scripting;

public class ScriptService : IScriptService
{
    private string _scriptsDirectory;
    private int ScriptCount { get; set; }

    private readonly Script _luaVm = new();
    private readonly ILogger<ScriptService> _logger;

    public static ScriptService Instance { get; private set; }

    public ScriptService(ILogger<ScriptService> logger)
    {
        _logger = logger;
        Instance = this;
    }

    public void Start()
    {
        try
        {
            var scriptDir =
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ??
                    throw new InvalidOperationException(), "Scripts");
                
            if (string.IsNullOrEmpty(scriptDir) || !Directory.Exists(scriptDir))
            {
                Directory.CreateDirectory(scriptDir);
                _logger.LogWarning("Missing directory: .\\Scripts has been created!");
            }

            _scriptsDirectory = scriptDir;

            RegisterFunctions();
            LoadScripts();

            _logger.LogDebug("{scriptCount} loaded successfully!", ScriptCount);

        }
        catch (Exception e)
        {
            _logger.LogError(
                "An exception occured while trying to load scripts!\n\nMessage:\n\t- {Message}\n\n Stack-Trace:\n\t- {StackTrace}",
                e.Message, e.StackTrace);
        }
    }

    public void RegisterFunction(string name, Func<object[], int> function) => _luaVm.Globals[name] = function;
    public void RegisterFunction(string name, Action<object[]> function) => _luaVm.Globals[name] = function;

    public int RunString(string script)
    {

        if (string.IsNullOrEmpty(script))
        {
            //Log.Error("Cannot RunString for a null script!");
            return 0;
        }

        try
        {
            _luaVm.DoString(script);
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

    private void RegisterFunctions()
    {
        RegisterFunction("call_lc_In", MiscFunc.SetCurrentLocationId);
        RegisterFunction("GetMonsterId", MonsterFunc.GetMonsterId);
        RegisterFunction("get_value", Player.get_value);
        //RegisterFunction("get_local", MiscFunc.GetLocal);
    }

    private void LoadScripts()
    {
        if (string.IsNullOrEmpty(_scriptsDirectory) || !Directory.Exists(_scriptsDirectory))
        {
            _logger.LogError("ScriptModule failed to load because the scripts directory is null or does not exist!");
            return;
        }

        var scriptPaths = Directory.GetFiles(_scriptsDirectory);
        var scriptTasks = new List<Task>();

        foreach (var path in scriptPaths)
        {
            var path1 = path;
            scriptTasks.Add(Task.Run(() =>
            {
                try
                {
                    _luaVm.DoFile(path1);
                }
                catch (Exception ex)
                {
                    if (ex is SyntaxErrorException or ScriptRuntimeException)
                    {
                        _logger.LogError("{path} could not be loaded!\n\nMessage: {message}\n)",
                            Path.GetFileName(path1),
                            ((InterpreterException)ex).DecoratedMessage.DecoratedMessageToString());
                    }
                    else
                    {
                        _logger.LogError("An exception occured while loading {path}!\n\nMessage: {message}\nStack-Trace: {stacktrace}\n",
                            Path.GetFileName(path1),
                            ((InterpreterException)ex).DecoratedMessage.DecoratedMessageToString(), ex.StackTrace);
                    }
                }
            }));

            ScriptCount++;
        }

        var t = Task.WhenAll(scriptTasks);

        try
        {
            t.Wait();
        }
        catch
        {
            // ignored
        }

        foreach (var task in scriptTasks.Where(task => task.IsFaulted))
        {
            if (task.Exception != null)
            {
                _logger.LogError(task.Exception.Message);
            }
        }
    }
}