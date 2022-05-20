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

using Serilog.Events;
using System.Collections.Generic;
using Spectre.Console;
using Navislamia.World;
using System.Threading;
using Navislamia.Game.DbLoaders;
using Navislamia.Database.Interfaces;
using System.Text;

namespace Navislamia.Game
{
    public class GameModule : IGameService
    {
        IConfigurationService configSVC;
        IWorldService worldSVC;
        IDatabaseService dbSVC;
        IScriptingService scriptSVC;
        INotificationService notificationSVC;
        IMapService mapSVC;
        INetworkService networkSVC;

        List<IRepository> worldRepositories;

        public GameModule() { }

        public GameModule(IConfigurationService configurationService, IWorldService contentService, INotificationService notificationService, IDatabaseService databaseService, 
            IScriptingService scriptingService, IMapService mapService, INetworkService networkService)
        {
            configSVC = configurationService;
            worldSVC = contentService;
            notificationSVC = notificationService;
            dbSVC = databaseService;
            scriptSVC = scriptingService;
            mapSVC = mapService;
            networkSVC = networkService;

            worldRepositories = new List<IRepository>();
        }

        public async Task<int> Start(string ip, int port, int backlog)
        {
            if (!configSVC.Get<bool>("skip_loading", "Scripts", false))
            {
                if (scriptSVC.Init() > 0)
                {
                    notificationSVC.WriteError("Failed to start script service!");

                    return 1;
                }

                notificationSVC.WriteSuccess(new string[] { $"Successfully started the script service!", $"[green]{scriptSVC.ScriptCount}[/] scripts loaded!" }, true);
            }
            else
                notificationSVC.WriteWarning("Script loading disabled!");

            if (!configSVC.Get<bool>("skip_loading", "Maps", false))
            {
                if (!mapSVC.Initialize($"{Directory.GetCurrentDirectory()}\\Maps"))
                {
                    notificationSVC.WriteError("Failed to start the map service!");

                    return 1;
                }

                notificationSVC.WriteSuccess(new string[] { $"Successfully started the map service!", $"[green]{mapSVC.MapCount.CX + mapSVC.MapCount.CY}[/] files loaded!" }, true);
            }
            else
                notificationSVC.WriteWarning("Map loading disabled!");

            if (!await loadDbRepositories())
            {
                // TODO: log this shit bruh
                return 1;
            }

            if (networkSVC.Initialize() > 0)
            {
                // TODO: log this shit bruh
                return 1;
            }

            int curTime = 0;
            int maxTime = 5000;
            while (!networkSVC.Ready)
            {
                if (curTime >= maxTime)
                {
                    notificationSVC.WriteError("Network service timed out!");
                    return 1;
                }

                curTime += 250;

                // Wait for the auth/upload servers to respond
                Thread.Sleep(250);
            }

            networkSVC.StartListener();

            return 0;
        }

        async Task<bool> loadDbRepositories()
        {
            List<Task<RepositoryLoader>> loadTasks = new List<Task<RepositoryLoader>>();

            loadTasks.Add(Task.Run(() => new MonsterLoader(notificationSVC, dbSVC.WorldConnection).Init()));
            loadTasks.Add(Task.Run(() => new StringLoader(notificationSVC, dbSVC.WorldConnection).Init()));

            try
            {
                while (loadTasks.Any())
                {
                    var task = await Task.WhenAny(loadTasks);

                    RepositoryLoader loader = task.Result;

                    foreach (IRepository repo in loader.Repositories)
                        worldRepositories.Add(repo);

                    loadTasks.Remove(task);
                }
            }
            catch (Exception ex) { }

            return true;
        }
    }
}
