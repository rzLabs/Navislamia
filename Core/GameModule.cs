using System;
using System.Threading.Tasks;
using System.Diagnostics;

using System.IO;
using System.Linq;
using System.Reflection;

using Configuration;
using Database;
using Navislamia.Data;
using Maps;
using Network;
using Notification;
using Scripting;

using Autofac;
using Serilog.Events;

namespace Navislamia.Game
{
    public class GameModule : Autofac.Module, IGameService
    {
        IContainer dependencies;
        IConfigurationService configSVC;
        IDatabaseService dbSVC;
        IDataService dataSVC;
        IScriptingService scriptSVC;
        INotificationService notificationSVC;
        IMapService mapSVC;
        INetworkService networkSVC;

        public GameModule() { }

        public GameModule(IConfigurationService configurationService, INotificationService notificationService)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<DatabaseModule>();
            builder.RegisterModule<NetworkModule>();
            builder.RegisterModule<ScriptModule>();
            builder.RegisterModule<MapModule>();

            dependencies = builder.Build();

            configSVC = configurationService;
            notificationSVC = notificationService;
            dataSVC = dependencies.Resolve<IDataService>();
            dbSVC = dependencies.Resolve<IDatabaseService>(new NamedParameter("configurationService", configSVC), new NamedParameter("notificationService", notificationSVC), new NamedParameter("dataService", dataSVC));
            scriptSVC = dependencies.Resolve<IScriptingService>(new NamedParameter("configurationService", configSVC), new NamedParameter("notificationService", notificationSVC));
            mapSVC = dependencies.Resolve<IMapService>();
            networkSVC = dependencies.Resolve<INetworkService>(new NamedParameter("configurationService", configSVC), new NamedParameter("notificationService", notificationSVC));
        }

        public int Start(string ip, int port, int backlog)
        {
            scriptSVC.Init();

            notificationSVC.WriteMarkup($"Successfully started and subscribed to the script service![green]\n\t-{ scriptSVC.ScriptCount } scripts loaded![/]", LogEventLevel.Debug);

            if (!configSVC.Get<bool>("skip_loading", "Maps", false))
            {
                mapSVC.Initialize($"{Directory.GetCurrentDirectory()}\\Maps", configSVC, scriptSVC, notificationSVC);

                notificationSVC.WriteMarkup("Successfully started and subscribed to the map service![green]\n\t- {mapSVC.MapCount.CX} maps loaded![/]", LogEventLevel.Debug);
            }
            else
                notificationSVC.WriteMarkup("[slowblink bold orange3]Map loading disabled![/]");

            dbSVC.Init();

            notificationSVC.WriteString("Successfully started and subscribed to the database service!", LogEventLevel.Debug);

            if (networkSVC.ConnectToAuth() > 0)
            {
                notificationSVC.WriteMarkup("[bold red]Network service failed to start![/]");
                return 1;
            }

            notificationSVC.WriteString("Successfully started and subscribed to the network service!", LogEventLevel.Debug);

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
