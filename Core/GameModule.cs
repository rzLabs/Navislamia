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
            notificationSVC.WriteConsoleLog("Starting game service...");

            notificationSVC.WriteConsoleLog("Starting script service...", null, LogEventLevel.Debug); 

            scriptSVC.Init();

            notificationSVC.WriteConsoleLog("Successfully started and subscribed to the script service!", null, LogEventLevel.Debug);

            if (!configSVC.Get<bool>("skip_loading", "Maps", false))
            {
                notificationSVC.WriteConsoleLog("Starting map service...", null, LogEventLevel.Debug);

                mapSVC.Initialize($"{Directory.GetCurrentDirectory()}\\Maps", configSVC, scriptSVC, notificationSVC);

                notificationSVC.WriteConsoleLog("Successfully started and subscribed to the map service!", null, LogEventLevel.Debug);
            }

            notificationSVC.WriteConsoleLog("Starting network service...", null, LogEventLevel.Debug);

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
