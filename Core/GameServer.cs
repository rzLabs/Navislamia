using System;
using System.Threading.Tasks;
using Navislamia.Configuration;
using Navislamia.LUA;
using Navislamia.Events;
using Navislamia.Maps;

namespace Navislamia.Core
{
    public class GameServer
    {
        public EventManager EventMgr = new EventManager();

        public ConfigurationManager ConfigMgr;
        public LuaManager LuaMgr;
        
        public GameServer(string configDirectory, string configName, string luaDirectory, string mapDirectory)
        {
            if (!string.IsNullOrEmpty(configDirectory) && !string.IsNullOrEmpty(configName))
                ConfigMgr = new ConfigurationManager(configDirectory, configName);
            else
                ConfigMgr = new ConfigurationManager();

            LuaMgr = new LuaManager();
        }

        public bool Initialize()
        {
            if (!ConfigMgr.Initialize())
                return false;

            if (!LuaMgr.Initialize(ConfigMgr.GetDirectory("Directory", "Scripts")))
                return false;

            // TODO: Db loading should occur here!

            if (!MapLoader.Initialize(ConfigMgr.GetDirectory("Directory", "Maps")))
                return false;

            return true;
        }
    }
}
