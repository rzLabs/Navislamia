using System;
using System.Threading.Tasks;
using System.Diagnostics;

using System.IO;
using System.Linq;
using System.Reflection;

using Configuration;
using Database;
using Maps;
using Network;
using Notification;
using Scripting;

using Autofac;
using Serilog.Events;

namespace Game
{
    public class GameModule : Autofac.Module, IGameService
    {
        IContainer dependencies;
        IConfigurationService configSVC;
        IDatabaseService dbSVC;
        IScriptingService scriptSVC;
        INotificationService notificationSVC;
        IMapService mapSVC;
        INetworkService networkSVC;

        public GameModule() { }

        public GameModule(IConfigurationService configurationService, INotificationService notificationService)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<DatabaseModule>();
            builder.RegisterModule<NetworkModule>();
            builder.RegisterModule<ScriptModule>();
            builder.RegisterModule<MapModule>();

            dependencies = builder.Build();

            configSVC = configurationService;
            notificationSVC = notificationService;
            dbSVC = dependencies.Resolve<IDatabaseService>(new NamedParameter("configurationService", configSVC), new NamedParameter("notificationService", notificationSVC));
            scriptSVC = dependencies.Resolve<IScriptingService>(new NamedParameter("configurationService", configSVC), new NamedParameter("notificationService", notificationSVC));
            mapSVC = dependencies.Resolve<IMapService>();
            networkSVC = dependencies.Resolve<INetworkService>(new NamedParameter("configurationService", configSVC), new NamedParameter("notificationService", notificationSVC));
        }

        public int Start(string ip, int port, int backlog)
        {
            scriptSVC.Init();

            notificationSVC.WriteConsoleLog("Successfully started and subscribed to the script service!\n\t-{0} scripts loaded!", new object[] { scriptSVC.ScriptCount }, LogEventLevel.Debug);

            if (!configSVC.Get<bool>("skip_loading", "Maps", false))
            {
                mapSVC.Initialize($"{Directory.GetCurrentDirectory()}\\Maps", configSVC, scriptSVC, notificationSVC);

                notificationSVC.WriteConsoleLog("Successfully started and subscribed to the map service!\n\t- {0} maps loaded!", new object[] { mapSVC.MapCount.CX }, LogEventLevel.Debug);
            }
            else
                notificationSVC.WriteConsoleLog("Map loading disabled!");

            dbSVC.Init();

            notificationSVC.WriteConsoleLog("Successfully started and subscribed to the database service!", null, LogEventLevel.Debug);

            networkSVC.Start();

            notificationSVC.WriteConsoleLog("Successfully started and subscribed to the network service!", null, LogEventLevel.Debug);

            return 0;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var configServiceTypes = Directory.EnumerateFiles(Environment.CurrentDirectory)
                .Where(filename => filename.Contains("Modules") && filename.EndsWith("Game.dll"))
                .Select(filepath => Assembly.LoadFrom(filepath))
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(IGameService).IsAssignableFrom(type) && type.IsClass));

            foreach (var configServiceType in configServiceTypes)
                builder.RegisterType(configServiceType).As<IGameService>();
        }
    }
}
