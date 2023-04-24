using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Navislamia.Database;
using Maps;
using Network;
using Navislamia.Notification;
using Scripting;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Database.Loaders;
using Navislamia.Database.Interfaces;

namespace Navislamia.Game
{
    public class GameModule : IGameService
    {
        private readonly IDatabaseService dbSVC;
        private readonly IScriptingService scriptSVC;
        private readonly INotificationService notificationSVC;
        private readonly IMapService mapSVC;
        private readonly INetworkService networkSVC;
        private readonly ScriptOptions _scriptOptions;
        private readonly MapOptions _mapOptions;
        private readonly List<IRepository> worldRepositories;

        public GameModule(INotificationService notificationService, IDatabaseService databaseService,
            IScriptingService scriptingService, IMapService mapService, INetworkService networkService, 
            IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions)
        {
            notificationSVC = notificationService;
            dbSVC = databaseService;
            scriptSVC = scriptingService;
            mapSVC = mapService;
            networkSVC = networkService;
            worldRepositories = new List<IRepository>();
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;

            if (_scriptOptions == null || _mapOptions == null)
            {
                throw new Exception("Script and/or Map options could not be loaded");
            }
        }

        public async Task<int> Start(string ip, int port, int backlog)
        {
            if (_scriptOptions.SkipLoading)
            {
                notificationSVC.WriteWarning("Script loading disabled!");
            }
            else
            {
                if (scriptSVC.Init() > 0)
                {
                    notificationSVC.WriteError("Failed to start script service!");

                    return 1;
                }

                notificationSVC.WriteSuccess(new string[] { $"Script service started successfully!", $"[green]{scriptSVC.ScriptCount}[/] scripts loaded!" }, true);
            }
           
            if (_mapOptions.SkipLoading)
            {
                notificationSVC.WriteWarning("Map loading disabled!");
            }
            else
            {
                if (!mapSVC.Initialize($"{Directory.GetCurrentDirectory()}\\Maps"))
                {
                    notificationSVC.WriteError("Failed to start the map service!");

                    return 1;
                }

                notificationSVC.WriteSuccess(new string[] { $"Map service started successfully!", $"[green]{mapSVC.MapCount.CX + mapSVC.MapCount.CY}[/] files loaded!" }, true);
            }

            if (!LoadDbRepositories())
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

        bool LoadDbRepositories()
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
