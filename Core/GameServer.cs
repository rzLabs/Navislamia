using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Navislamia.Data;
using Navislamia.Configuration;
using Navislamia.LUA;
using Navislamia.Maps;
using Navislamia.Network;
using Navislamia.Utilities;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks;

namespace Navislamia.Core
{
    public class GameServer
    {
        public ConfigurationManager ConfigMgr;
        public LuaManager LuaMgr;

        GameNetwork network;

        LoggingLevelSwitch logLevel = new LoggingLevelSwitch();

        Stopwatch watch = new Stopwatch();

        public GameServer(string configDirectory, string configName, string luaDirectory, string mapDirectory)
        {
            if (!string.IsNullOrEmpty(configDirectory) && !string.IsNullOrEmpty(configName))
                ConfigMgr = new ConfigurationManager(configDirectory, configName);
            else
                ConfigMgr = new ConfigurationManager();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(logLevel)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}", theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                .WriteTo.File(".\\Logs\\log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            LuaMgr = new LuaManager();
        }

        public void Start()
        {
            Log.Information("Starting server...");

            watch.Restart();

            if (!ConfigMgr.Initialize()) {
                Log.Fatal("ConfigurationManager failed to initialize!");
                return;
            }

            watch.Stop();

            setLogLevel();

            Log.Verbose("\t- {count} settings loaded from {name} in {time}", ConfigMgr.Count, "Configuration.json", StringExt.MilisecondsToString(watch.ElapsedMilliseconds));

            //TODO: if initDatabase

            if (loadAssets())
            {
                //TODO: if startWorld
                if (startNetwork())
                {
                    Log.Information("Server started!");
                    return;
                }
            }

            // We should never make it here!
            Log.Fatal("Server failed to start!");
        }

        public void Stop() => throw new NotImplementedException();

        void setLogLevel() =>
            logLevel.MinimumLevel = (LogEventLevel)ConfigMgr["minimum-level", "Logs"];

        bool initDatabase() => throw new NotImplementedException();

        bool loadAssets()
        {
            watch.Restart();

            if (!LuaMgr.Initialize(ConfigMgr.GetDirectory("Directory", "Scripts"))) {
                Log.Fatal("LuaManager failed to initialize!");
                return false;
            }

            watch.Stop();

            // TODO: Db loading should occur here!

            if (ConfigMgr.Get<bool>("skip_loading", "Maps"))
            {
                Log.Information("Map loading disabled!");

                goto returntrue;
            }

            Log.Information("Loading maps...");

            watch.Restart();

            if (!MapLoader.Initialize(ConfigMgr.GetDirectory("Directory", "Maps"))) {
                Log.Fatal("MapLoader failed to initialize!");
                return false;
            }

            watch.Stop();

            Log.Information("Map loading completed in {time}", StringExt.MilisecondsToString(watch.ElapsedMilliseconds));

            returntrue:

            return true;
        }

        void initWorld() => throw new NotImplementedException();

        bool startNetwork()
        {
            network = new GameNetwork();

            if (!network.Start())
            {
                Log.Fatal("Failed to start network!");
                return false;
            }

            return true;
        }
    }
}
