using System;
using System.Threading.Tasks;

using Navislamia.Data;
using Navislamia.Configuration;
using Navislamia.LUA;
using Navislamia.Events;
using Navislamia.Maps;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks;

namespace Navislamia.Core
{
    public class GameServer
    {
        public EventManager EventMgr = new EventManager();

        public ConfigurationManager ConfigMgr;
        public LuaManager LuaMgr;

        public ILogger Logger;

        LoggingLevelSwitch logLevel = new LoggingLevelSwitch();

        public GameServer(string configDirectory, string configName, string luaDirectory, string mapDirectory)
        {
            if (!string.IsNullOrEmpty(configDirectory) && !string.IsNullOrEmpty(configName))
                ConfigMgr = new ConfigurationManager(configDirectory, configName);
            else
                ConfigMgr = new ConfigurationManager();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(logLevel)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(".\\Logs\\log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Logger = Log.ForContext<GameServer>();

            LuaMgr = new LuaManager();
        }

        public void Start()
        {
            Logger.Information("Starting server...");

            if (loadConfig()) {
                setLogLevel();
                
                //TODO: if initDatabase

                if (loadAssets()) {
                    //TODO: if startWorld
                    //TODO: if startNetwork

                    Logger.Information("Server started!");
                    return;
                }
            }

            Logger.Fatal("Server failed to start!");     
        }

        public void Stop() => throw new NotImplementedException();

        bool loadConfig()
        {
            if (!ConfigMgr.Initialize()) { 
                Logger.Fatal("ConfigurationManager failed to initialize!");
                return false;
            }

            return true;
        }

        void setLogLevel() =>
            logLevel.MinimumLevel = (LogEventLevel)ConfigMgr["minimum-level", "Logs"];

        bool initDatabase() => throw new NotImplementedException();

        bool loadAssets()
        {
            if (!LuaMgr.Initialize(ConfigMgr.GetDirectory("Directory", "Scripts"))) {
                Logger.Fatal("LuaManager failed to initialize!");
                return false;
            }

            Logger.Information("{0} LUA Scripts loaded!", LuaMgr.ScriptCount);

            // TODO: Db loading should occur here!

            Logger.Information("Loading maps...");

            if (!MapLoader.Initialize(ConfigMgr.GetDirectory("Directory", "Maps"))) {
                Logger.Fatal("MapLoader failed to initialize!");
                return false;
            }

            return true;
        }

        void initWorld() => throw new NotImplementedException();

        bool startNetwork() => throw new NotImplementedException();
    }
}
