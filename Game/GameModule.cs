using System;
using System.Threading.Tasks;
using System.IO;
using Navislamia.Database;
using Maps;
using Navislamia.Notification;
using System.Collections.Generic;
using Navislamia.World;
using System.Threading;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Database.Loaders;
using Navislamia.Database.Interfaces;
using Navislamia.Network;
using Navislamia.Scripting;

namespace Navislamia.Game
{
    public class GameModule : IGameModule
    {
        private readonly IWorldService _worldService;
        private readonly IDatabaseService _databaseService;
        private readonly IScriptingModule _scriptingModule;
        private readonly INotificationService _notificationService;
        private readonly IMapService _mapService;
        private readonly INetworkModule _networkModule;
        private readonly ScriptOptions _scriptOptions;
        private readonly MapOptions _mapOptions;
        private readonly ServerOptions _serverOptions;

        private List<IRepository> _worldRepositories;

        public GameModule() { }

        public GameModule(IWorldService worldService, INotificationService notificationService,
            IDatabaseService databaseService, IScriptingModule scriptingService, IMapService mapService,
            INetworkModule networkModule, IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions,
            IOptions<ServerOptions> serverOptions)
        {
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;
            _serverOptions = serverOptions.Value;
            _worldService = worldService;
            _notificationService = notificationService;
            _databaseService = databaseService;
            _scriptingModule = scriptingService;
            _mapService = mapService;
            _networkModule = networkModule;

            _worldRepositories = new List<IRepository>();
        }

        public async Task Start(string ip, int port, int backlog)
        {
            LoadScripts(_scriptOptions.SkipLoading);
            StartMapService(_mapOptions.SkipLoading);
            LoadDbRepositories();
            _networkModule.Initialize();

            var curTime = DateTime.UtcNow;
            var maxTime = DateTime.UtcNow.AddMinutes(5);
            
            while (!_networkModule.IsReady())
            {
                if (curTime >= maxTime)
                {
                    _notificationService.WriteError("Network service timed out!");
                    return;
                }

                // Wait for the auth/upload servers to respond
                Thread.Sleep(250);
                curTime = curTime.AddMilliseconds(250);
            }

            _networkModule.StartListener();
            _networkModule.StartListener();
        }

        private void StartMapService(bool skip)
        {
            if (skip)
            {
                _notificationService.WriteWarning("Map loading disabled!");
                return;
            }

            if (!_mapService.Initialize($"{Directory.GetCurrentDirectory()}\\Maps"))
            {
                _notificationService.WriteError("Failed to start the map service!");
            }

            _notificationService.WriteSuccess(
                new[]
                {
                    "Map service started successfully!",
                    $"[green]{_mapService.MapCount.CX + _mapService.MapCount.CY}[/] files loaded!"
                }, true);
        }

        private void LoadScripts(bool skip)
        {
            if (skip)
            {
                _notificationService.WriteWarning("Script loading disabled!");
                return;
            }

            _scriptingModule.Init();
        }

        private void LoadDbRepositories()
        {
            var loaders = new List<IRepositoryLoader>
            {
                new MonsterLoader(_notificationService, _databaseService),
                new PetLoader(_notificationService, _databaseService),
                new ItemLoader(_notificationService, _databaseService),
                new NPCLoader(_notificationService, _databaseService),
                new StringLoader(_notificationService, _databaseService)
            };

            foreach(var loader in loaders)
            {
                try
                {
                    loader.Init();
                }
                catch (Exception e)
                {
                   _notificationService.WriteError($"{loader.GetType().Name} failed to load!");
                   throw new Exception($"{loader.GetType().Name} failed to load!");
                }

                _worldRepositories.AddRange(loader.Repositories);
            }

            _notificationService.WriteNewLine(); 
        }
    }
}
