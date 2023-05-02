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
        private readonly IDatabaseService _dbService;
        private readonly IScriptingModule _scriptingModule;
        private readonly INotificationService _notificationService;
        private readonly IMapService _mapService;
        private readonly INetworkService _networkService;
        private readonly ScriptOptions _scriptOptions;
        private readonly MapOptions _mapOptions;
        private readonly ServerOptions _serverOptions;

        private List<IRepository> _worldRepositories;

        public GameModule() { }

        public GameModule(IWorldService worldService, INotificationService notificationService,
            IDatabaseService dbService, IScriptingModule scriptingModule, IMapService mapService,
            INetworkService networkService, IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions, IOptions<ServerOptions> serverOptions)
        {
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;
            _serverOptions = serverOptions.Value;
            _worldService = worldService;
            _notificationService = notificationService;
            _dbService = dbService;
            _scriptingModule = scriptingModule;
            _mapService = mapService;
            _networkService = networkService;

            _worldRepositories = new List<IRepository>();
        }

        public async Task Start(string ip, int port, int backlog)
        {
            _notificationService.WriteWarning(_serverOptions.Name);
            LoadScripts(_scriptOptions.SkipLoading);
            StartMapService(_mapOptions.SkipLoading);
            LoadDbRepositories();
            _networkService.Initialize();

            var curTime = DateTime.UtcNow;
            var maxTime = DateTime.UtcNow.AddMinutes(5);
            
            while (!_networkService.Ready)
            {
                if (curTime >= maxTime)
                {
                    _notificationService.WriteError("Network service timed out!");
                    return;
                }

                curTime = curTime.AddMilliseconds(250);

                // Wait for the auth/upload servers to respond
                Thread.Sleep(250);
            }

            _networkService.StartListener();
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
            var loaders = new List<IRepositoryLoader>();

            loaders.Add(new MonsterLoader(_notificationService, _dbService));
            loaders.Add(new PetLoader(_notificationService, _dbService));
            loaders.Add(new ItemLoader(_notificationService, _dbService));
            loaders.Add(new NPCLoader(_notificationService, _dbService));
            loaders.Add(new StringLoader(_notificationService, _dbService));

            for (int i = 0; i < loaders.Count; i++)
            {
               if (loaders[i].Init() is null)
               {
                   _notificationService.WriteError($"{loaders[i].GetType().Name} failed to load!");
                   throw new Exception($"{loaders[i].GetType().Name} failed to load!");
               }

               _worldRepositories.AddRange(loaders[i].Repositories);
            }

            _notificationService.WriteNewLine(); 
        }
    }
}
