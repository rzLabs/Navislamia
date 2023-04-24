using System;
using System.Threading.Tasks;
using System.IO;

using Configuration;
using Navislamia.Database;
using Maps;
using Network;
using Navislamia.Notification;
using Scripting;

using Serilog.Events;
using System.Collections.Generic;
using Navislamia.World;
using System.Threading;
using Navislamia.Database.Loaders;
using Navislamia.Database.Interfaces;

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
            if (!configSVC.Get<bool>("skip_loading", "scripts", false))
            {
                if (scriptSVC.Init() > 0)
                {
                    notificationSVC.WriteError("Failed to start script service!");

                    return 1;
                }

                notificationSVC.WriteSuccess(new string[] { $"Script service started successfully!", $"[green]{scriptSVC.ScriptCount}[/] scripts loaded!" }, true);
            }
            else
                notificationSVC.WriteWarning("Script loading disabled!");

            if (!configSVC.Get<bool>("skip_loading", "maps", false))
            {
                if (!mapSVC.Initialize($"{Directory.GetCurrentDirectory()}\\Maps"))
                {
                    notificationSVC.WriteError("Failed to start the map service!");

                    return 1;
                }

                notificationSVC.WriteSuccess(new string[] { $"Map service started successfully!", $"[green]{mapSVC.MapCount.CX + mapSVC.MapCount.CY}[/] files loaded!" }, true);
            }
            else
                notificationSVC.WriteWarning("Map loading disabled!");

            if (!loadDbRepositories())
                return 1;

            if (networkSVC.Initialize() > 0)
                return 1;

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

        bool loadDbRepositories()
        {
            var loaders = new List<IRepositoryLoader>();

            loaders.Add(new MonsterLoader(notificationSVC, dbSVC));
            loaders.Add(new PetLoader(notificationSVC, dbSVC));
            loaders.Add(new ItemLoader(notificationSVC, dbSVC));
            loaders.Add(new NPCLoader(notificationSVC, dbSVC));
            loaders.Add(new StringLoader(notificationSVC, dbSVC));

            for (int i = 0; i < loaders.Count; i++)
            {
               if (loaders[i].Init() is null)
                {
                    notificationSVC.WriteError($"{loaders[i].GetType().Name} failed to load!");
                    return false;
                }

                worldRepositories.AddRange(loaders[i].Repositories);
            }

            notificationSVC.WriteNewLine(); // Write a blank line that will have a newline appended

            return true;
        }
    }
}
