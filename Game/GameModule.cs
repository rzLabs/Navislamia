using System.Threading.Tasks;
using System.IO;
using Navislamia.Database;
using Maps;
using Navislamia.Notification;
using Scripting;
using System.Collections.Generic;
using Navislamia.World;
using System.Threading;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Database.Loaders;
using Navislamia.Database.Interfaces;
using Navislamia.Network;

namespace Navislamia.Game
{
    public class GameModule : IGameModule
    {
        IWorldService worldSVC;
        IDatabaseService dbSVC;
        IScriptingService scriptSVC;
        INotificationService notificationSVC;
        IMapService mapSVC;
        private readonly INetworkModule _networkModule;
        private readonly ScriptOptions _scriptOptions;
        private readonly MapOptions _mapOptions;
        
        List<IRepository> worldRepositories;

        public GameModule() { }

        public GameModule(IWorldService contentService, INotificationService notificationService,
            IDatabaseService databaseService, IScriptingService scriptingService, IMapService mapService,
            INetworkModule networkModule, IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions)
        {
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;
            worldSVC = contentService;
            notificationSVC = notificationService;
            dbSVC = databaseService;
            scriptSVC = scriptingService;
            mapSVC = mapService;
            _networkModule = networkModule;

            worldRepositories = new List<IRepository>();
        }

        public async Task Start(string ip, int port, int backlog)
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
                    return;
                }
                
                notificationSVC.WriteSuccess(new [] { "Script service started successfully!", $"[green]{scriptSVC.ScriptCount}[/] scripts loaded!" }, true);
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
                }

                notificationSVC.WriteSuccess(new [] { "Map service started successfully!", $"[green]{mapSVC.MapCount.CX + mapSVC.MapCount.CY}[/] files loaded!" }, true);
            }

            if (!LoadDbRepositories())
            {
                return;
            }

            if (_networkModule.Initialize() > 0)
            {
                return;
            }

            int curTime = 0;
            int maxTime = 5000;
            
            while (!_networkModule.IsReady())
            {
                if (curTime >= maxTime)
                {
                    notificationSVC.WriteError("Network service timed out!");
                    return;
                }

                curTime += 250;

                // Wait for the auth/upload servers to respond
                Thread.Sleep(250);
            }

            _networkModule.StartListener();
        }

        private bool LoadDbRepositories()
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
