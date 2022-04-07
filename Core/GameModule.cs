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
using System.Collections.Generic;

namespace Navislamia.Game
{
    public class GameModule : IGameService
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

        public GameModule(List<object> dependencies)
        {
            configSVC = dependencies.Find(d => d is IConfigurationService) as IConfigurationService;
            notificationSVC = dependencies.Find(d => d is INotificationService) as INotificationService;

            dataSVC = new DataModule();
            dbSVC = new DatabaseModule(new List<object>() { configSVC, notificationSVC, dataSVC });
            scriptSVC = new ScriptModule(new List<object>() { configSVC, notificationSVC });
            mapSVC = new MapModule(new List<object>() { configSVC, notificationSVC, scriptSVC });
            networkSVC = new NetworkModule(new List<object>() { configSVC, notificationSVC });
        }

        public int Start(string ip, int port, int backlog)
        {
            if (!configSVC.Get<bool>("skip_loading", "Scripts", false))
            {
                scriptSVC.Init();

                notificationSVC.WriteMarkup($"Successfully started and subscribed to the script service![green]\n\t-{ scriptSVC.ScriptCount } scripts loaded![/]", LogEventLevel.Debug);
            }
            else
                notificationSVC.WriteMarkup("[slowblink bold orange3]Script loading disabled![/]");


            if (!configSVC.Get<bool>("skip_loading", "Maps", false))
            {
                mapSVC.Initialize($"{Directory.GetCurrentDirectory()}\\Maps");

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
    }
}
