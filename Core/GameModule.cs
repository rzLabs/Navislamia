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

using Serilog.Events;
using System.Collections.Generic;
using Spectre.Console;

namespace Navislamia.Game
{
    public class GameModule : IGameService
    {
        IConfigurationService configSVC;
        IDatabaseService dbSVC;
        IDataService dataSVC;
        IScriptingService scriptSVC;
        INotificationService notificationSVC;
        IMapService mapSVC;
        INetworkService networkSVC;

        public GameModule() { }

        public GameModule(IConfigurationService configurationService, INotificationService notificationService, IDatabaseService databaseService, IDataService dataService, 
            IScriptingService scriptingService, IMapService mapService, INetworkService networkService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;
            dbSVC = databaseService;
            dataSVC = dataService;
            scriptSVC = scriptingService;
            mapSVC = mapService;
            networkSVC = networkService;
        }

        public int Start(string ip, int port, int backlog)
        {
            int result = 0;

            if (!configSVC.Get<bool>("skip_loading", "Scripts", false))
            {
                scriptSVC.Init();

                notificationSVC.WriteSuccess(new string[] { $"Successfully started the script service!", $"[green]{scriptSVC.ScriptCount}[/] scripts loaded!" }, true);
            }
            else
                notificationSVC.WriteWarning("Script loading disabled!");

            if (!configSVC.Get<bool>("skip_loading", "Maps", false))
            {
                mapSVC.Initialize($"{Directory.GetCurrentDirectory()}\\Maps");

                notificationSVC.WriteSuccess(new string[] { $"Successfully started the map service!", $"[green]{mapSVC.MapCount.CX + mapSVC.MapCount.CY}[/] files loaded!" }, true);
            }
            else
                notificationSVC.WriteWarning("Map loading disabled!");

            dbSVC.Init();

            if (networkSVC.ConnectToAuth() > 0)
            {
                notificationSVC.WriteError("Failed to connect to the auth server!");
                result = 1;
            }

            if (networkSVC.ConnectToUpload() > 0)
            {
                notificationSVC.WriteError("Failed to connect to the upload server!");
                result = 1;
            }

            return result;
        }
    }
}
